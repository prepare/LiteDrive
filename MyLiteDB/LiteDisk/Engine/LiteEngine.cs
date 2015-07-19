using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LiteDB
{
    /// <summary>
    /// The LiteDB engine. Used for create a LiteDB instance and use all storage resoures. It's the database connection engine.
    /// </summary>
    public partial class LiteEngine : IDisposable
    {
        public ConnectionString ConnectionString { get; private set; }

        internal RecoveryService Recovery { get; private set; }

        internal CacheService Cache { get; private set; }

        internal DiskService Disk { get; private set; }

        internal PageService Pager { get; private set; }

        internal RedoService Redo { get; private set; }

        internal TransactionService Transaction { get; private set; }

        internal IndexService Indexer { get; private set; }

        internal DataService Data { get; private set; }

        internal CollectionService Collections { get; private set; }

        /// <summary>
        /// Starts LiteDB engine. Open database file or create a new one if not exits
        /// </summary>
        /// <param name="connectionString">Full filename or connection string</param>
        public LiteEngine(string connectionString)
        {
            this.ConnectionString = new ConnectionString(connectionString);

            if (!File.Exists(ConnectionString.Filename))
                CreateNewDatabase(ConnectionString);

            this.Recovery = new RecoveryService(this.ConnectionString);

            this.Recovery.TryRecovery();

            this.Disk = new DiskService(this.ConnectionString);

            this.Cache = new CacheService(this.Disk);

            this.Pager = new PageService(this.Disk, this.Cache);

            this.Redo = new RedoService(this.Recovery, this.Cache, this.ConnectionString.JournalEnabled);

            this.Indexer = new IndexService(this.Cache, this.Pager);

            this.Transaction = new TransactionService(this.Disk, this.Cache, this.Redo);

            this.Data = new DataService(this.Disk, this.Cache, this.Pager);

            this.Collections = new CollectionService(this.Pager, this.Indexer);
        }

        #region Collections

        /// <summary>
        /// Get a collection using a strong typed POCO class. If collection does not exits, create a new one.
        /// </summary>
        /// <param name="name">Collection name (case insensitive)</param>
        public Collection GetCollection(string name)
        {
            return new Collection(this, name);
        }

        ///// <summary>
        ///// Get a collection using a generic BsonDocument. If collection does not exits, create a new one.
        ///// </summary>
        ///// <param name="name">Collection name (case insensitive)</param>
        //public Collection<BsonDocument> GetCollection(string name)
        //{
        //    return new Collection<BsonDocument>(this, name);
        //}

        /// <summary>
        /// Drop a collection, including all inside documents. Runs outside a transaction - there is no rollback
        /// </summary>
        /// <param name="name">Collection name (case insensitive)</param>
        public bool DropCollection(string name)
        {
            return this.Collections.Drop(name);
        }

        ///// <summary>
        ///// Get all collections name inside this database.
        ///// </summary>
        //public string[] GetCollections()
        //{
        //    return this.Collections.GetAll().Select(x => x.CollectionName).ToArray();
        //}

        #endregion

        #region UserVersion

        /// <summary>
        /// Get or set database version. It's used when need store data file version for check old versions before update.
        /// </summary>
        public int UserVersion
        {
            get { return this.Cache.Header.UserVersion; }
            set
            {
                if (this.Cache.Header.UserVersion != value)
                {
                    this.Transaction.Begin();

                    try
                    {
                        this.Cache.Header.UserVersion = value;
                        this.Cache.Header.IsDirty = true;

                        this.Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        this.Transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region MaxFileLength

        /// <summary>
        /// Get or set database max datafile length. Minumum is 256Kb. Default is long.MaxValue.
        /// </summary>
        public long MaxFileLength
        {
            get { return this.Cache.Header.MaxFileLength; }
            set
            {
                if (value < (256 * 1024)) throw new ArgumentException("MaxFileLength must be bigger than 262.144 (256Kb)");

                if (this.Cache.Header.MaxFileLength != value)
                {
                    this.Transaction.Begin();

                    try
                    {
                        this.Cache.Header.MaxFileLength = value;
                        this.Cache.Header.IsDirty = true;

                        if (this.Cache.Header.MaxPageID > this.Cache.Header.LastPageID) throw new ArgumentException("File size is bigger than " + value);

                        this.Transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        this.Transaction.Rollback();
                        throw ex;
                    }
                }
            }
        }

        #endregion

        #region Files Storage

        //private FileStorage _files = null;

        /// <summary>
        /// Returns a special collection for storage files/stream inside datafile
        /// </summary>
        //public FileStorage FileStorage
        //{
        //    get { return _files ?? (_files = new FileStorage(this)); }
        //}

        #endregion

        #region Transaction

        /// <summary>
        /// Starts a new transaction. After this command, all write operations will be first in memory and will persist on disk
        /// only when call Commit() method. If any error occurs, a Rollback() method will run.
        /// </summary>
        public void BeginTrans()
        {
            this.Transaction.Begin();
        }

        /// <summary>
        /// Persist all changes on disk.
        /// </summary>
        public void Commit()
        {
            this.Transaction.Commit();
        }

        /// <summary>
        /// Cancel all write operations and keep datafile as is before BeginTrans() called
        /// </summary>
        public void Rollback()
        {
            this.Transaction.Rollback();
        }

        #endregion

        #region Statics methods

        /// <summary>
        /// Create a empty database ready to be used using connectionString as parameters
        /// </summary>
        private static void CreateNewDatabase(ConnectionString connectionString)
        {
            using (var stream = File.Create(connectionString.Filename))
            {
                using (var writer = new BinaryWriter(stream))
                {
                    // creating header + master collection
                    DiskService.WritePage(writer, new HeaderPage { PageID = 0, LastPageID = 1 });
                    DiskService.WritePage(writer, new CollectionPage { PageID = 1, CollectionName = "_master" });
                }
            }
        }

        #endregion

        public void Dispose()
        {
            Disk.Dispose();
        }
    }
}
