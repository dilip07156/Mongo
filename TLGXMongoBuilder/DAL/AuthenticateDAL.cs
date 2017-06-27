using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DAL
{
    public class AuthenticateDAL : IDisposable
    {
        protected static IMongoDatabase _database;

        public void Dispose()
        {
        }

        public static int DefaultSecondsUntilTokenExpires = int.Parse(System.Configuration.ConfigurationManager.AppSettings["TokenExpirySeconds"]);

        public bool SaveToken(DataContracts.TokenContract nTokenRecord)
        {
            try
            {
                //using (ETNSupportEntities context = new ETNSupportEntities())
                //{
                //    //remove any existing token.
                //    var token = context.TokenMapSvcs.SingleOrDefault(t => t.UserId == UserId);
                //    if (token != null)
                //    {
                //        context.TokenMapSvcs.Remove(token);
                //        context.SaveChanges();
                //    }

                //    //Save New Token
                //    context.TokenMapSvcs.Add(new TokenMapSvc { UserId = UserId, Token = Token, CreateDate = DateTime.Now });
                //    context.SaveChanges();
                //    return true;
                //}

                //Save Data using MongoDB
                _database = MongoDBHandler.mDatabase();

                //Get Token Collection
                var collection = _database.GetCollection<DataContracts.TokenContract>("TokenMapSvc");

                //Delete any existing records
                collection.DeleteMany(p => p.UserID == nTokenRecord.UserID.ToString());

                //Insert a new records
                collection.InsertOne(nTokenRecord);

                return true;

            }
            catch
            {
                return false;
            }

        }

        public DataContracts.TokenContract GetToken(string Token)
        {
            //using (ETNSupportEntities context = new ETNSupportEntities())
            //{
            //    var token = context.TokenMapSvcs.SingleOrDefault(t => t.Token == Token);
            //    if (token != null)
            //    {
            //        var span = DateTime.Now - token.CreateDate;
            //        if (span.TotalSeconds > DefaultSecondsUntilTokenExpires)
            //        {
            //            context.TokenMapSvcs.Remove(token);
            //            context.SaveChanges();
            //            return false;
            //        }
            //        else
            //        {
            //            return true;
            //        }
            //    }
            //    else
            //    {
            //        return true;
            //    }
            //}
            _database = MongoDBHandler.mDatabase();

            //Get Token Collection
            var collection = _database.GetCollection<DataContracts.TokenContract>("TokenMapSvc");

            //Get Specific Record
            var nTokenRecord = collection.Find(p => p.Token == Token).SingleOrDefault();

            return nTokenRecord;

            

        }

        public bool UpdateToken(DataContracts.TokenContract nTokenRecord)
        {
            try
            {
                //Save Data using MongoDB
                _database = MongoDBHandler.mDatabase();

                //Get Token Collection
                var collection = _database.GetCollection<DataContracts.TokenContract>("TokenMapSvc");

                var filter = Builders<DataContracts.TokenContract>.Filter.Where(_ => _.UserID == nTokenRecord.UserID);
                var update = Builders<DataContracts.TokenContract>.Update.Set(_ => _.ExpiryDate, nTokenRecord.ExpiryDate);
                var options = new FindOneAndUpdateOptions<DataContracts.TokenContract>();

                collection.FindOneAndUpdate(filter, update, options);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //public List<DataContracts.UserRoles> GetRoles(Guid UserId)
        //{
        //    using (ETNSupportEntities context = new ETNSupportEntities())
        //    {
        //        var roles = from a in context.aspnet_Applications
        //                    join r in context.aspnet_Roles on a.ApplicationId equals r.ApplicationId
        //                    join ur in context.aspnet_UsersInRoles on r.RoleId equals ur.RoleId
        //                    where a.ApplicationName == "voyager" && ur.UserId == UserId
        //                    select new DataContracts.UserRoles { RoleId = r.RoleId, RoleName = r.RoleName };
        //        return roles.ToList();
        //    }
        //}
    }
}
