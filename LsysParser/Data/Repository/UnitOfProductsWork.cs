using LsysParser.Data.Model;
using LsysParser.Data.Repository;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LsysParser.Data
{
    class UnitOfProductsWork : IDisposable
    {
        private readonly ProductsContext context;

        public UnitOfProductsWork()
        {
            context = new ProductsContext(GetConnetionString());

            Categorys = new CategoryRepository(context);
            Brands = new BrandRepository(context);
            Products = new ProductRepository(context);
            Propertyes = new PropertyRepository(context);
            PropertyNames = new PropertyNameRepository(context);
            PropertyValues = new PropertyValueRepository(context);
            Files = new FileRepository(context);
            Errors = new ErrorRepository(context);
        }

        public CategoryRepository Categorys { get; private set; }
        public BrandRepository Brands { get; private set; }
        public ProductRepository Products { get; private set; }
        public PropertyRepository Propertyes { get; private set; }
        public PropertyNameRepository PropertyNames { get; private set; }
        public PropertyValueRepository PropertyValues { get; private set; }
        public FileRepository Files { get; private set; }
        public ErrorRepository Errors { get; private set; }

        public ProductsContext Context
        {
            get
            {
                return context;
            }
        }

        public string GetConnetionString()
        {
            var ini = new Ini("conf.ini");

            var efBuilder = new System.Data.Entity.Core.EntityClient.EntityConnectionStringBuilder();
            efBuilder.Metadata = "res://*/Data.Model.ProductsModel.csdl|res://*/Data.Model.ProductsModel.ssdl|res://*/Data.Model.ProductsModel.msl";
            efBuilder.Provider = "System.Data.SqlClient";

            var sb = new SqlConnectionStringBuilder();
            sb.DataSource = ini.GetValue("server");
            sb.InitialCatalog = ini.GetValue("database");
            sb.IntegratedSecurity = true;
            sb.MultipleActiveResultSets = true;
            sb["App"] = "EntityFramework";

            efBuilder.ProviderConnectionString = sb.ToString();

            return efBuilder.ToString();
        }

        public void Sql(string sql)
        {
            context.Database.ExecuteSqlCommand(sql);
        }

        public void Dispose()
        {
            context.Dispose();
        }

        public void Complete()
        {
            context.SaveChanges();
        }
    }
}
