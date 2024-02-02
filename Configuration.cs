namespace Blog
{
	public class Configuration
	{
		public static string JwtKey = "KFzQUb~BOCPtj6}f89ZFWo27qpxsYmX$CfIfE4hR5It4dCEmyE^GUQfXJapAf3hd";
		public static string ApiKeyName = "api_key";
		public static string ApiKey = "curso_api_ceb3d85e1b83b078f1f7d74";
		public static SmtpConfiguration Smtp = new();

		public class SmtpConfiguration
		{
            public string Host { get; set; }
			public int Port { get; set; } = 25;
            public string UserName { get; set; }
            public string Password { get; set; }	
        }
	}
}
