using MongoDB.Driver;

namespace RuiSantos.ZocDoc.Data.Mongodb;

public class MongoSettings
{
	public string Host { get; set; } 
	public int Port { get; set; }
	public string Schema { get; set; }

	public MongoSettings()
	{
		this.Host = string.Empty;
		this.Port = 27017;
		this.Schema = string.Empty;
	}

	internal MongoClientSettings ToMongoClientSettings()
	{
		var settings = MongoClientSettings.FromConnectionString($"mongodb://{Host}:{Port}");
		settings.LinqProvider = MongoDB.Driver.Linq.LinqProvider.V3;
		return settings;
	}
}

