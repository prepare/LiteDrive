﻿using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LiteDB.Shell.Commands
{
    class CollectionExec : BaseCollection, IShellCommand
    {
        public bool IsCommand(StringScanner s)
        {
            return this.IsCollectionCommand(s, "exec");
        }

        public void Execute(LiteEngine db, StringScanner s, Display display)
        {
            if (db == null) throw new LiteException("No database");

            var col = this.ReadCollection(db, s);
            var query = s.Match("{") ? Query.All() : this.ReadQuery(s);
            var code = DynamicCode.GetCode(s);

            var docs = col.Find(query).ToArray();

            try
            {
                db.BeginTrans();

                foreach (var doc in docs)
                {
                    code(doc["_id"].RawValue, doc, col, db);
                }

                db.Commit();

                display.WriteBson(docs.Length);
            }
            catch (Exception ex)
            {
                db.Rollback();
                throw ex;
            }
        }
    }

    internal class DynamicCode
    {
        private const string CODE_TEMPLATE = @"
using System; 
using LiteDB;

public class Program {
    public static void DoWork(
        object id, 
        BsonDocument doc, 
        Collection<BsonDocument> col, 
        LiteEngine db) { [code] }
}";

        public static Action<object, BsonDocument, Collection<BsonDocument>, LiteEngine> GetCode(StringScanner s)
        {
            var str = s.Scan(@".*");
            var code = CODE_TEMPLATE.Replace("[code]", str);
            var provider = new CSharpCodeProvider();
            var parameters = new CompilerParameters();

            parameters.ReferencedAssemblies.Add("LiteDB.dll");
            parameters.GenerateInMemory = true;
            parameters.GenerateExecutable = false;

            var results = provider.CompileAssemblyFromSource(parameters, code);

            if (results.Errors.HasErrors)
            {
                var err = new StringBuilder();
                foreach (CompilerError error in results.Errors)
                {
                    err.AppendLine(String.Format("Error ({0}): {1}", error.ErrorNumber, error.ErrorText));
                }
                throw new InvalidOperationException(err.ToString().Trim());
            }

            var assembly = results.CompiledAssembly;
            var program = assembly.GetType("Program");
            var method = program.GetMethod("DoWork");

            return new Action<object, BsonDocument, Collection<BsonDocument>, LiteEngine>((id, doc, col, db) =>
            {
                method.Invoke(null, new object[] { id, doc, col, db });
            });
        }
    }
}
