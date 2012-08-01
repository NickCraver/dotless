namespace dotless.Core
{
    using System.Web;
    using configuration;
    using Microsoft.Practices.ServiceLocation;
    using System.IO;
    using dotless.Core.Input;
    using dotless.Core.Response;
    using System;

    public class LessCssHttpHandler : IHttpHandler
    {
        public IServiceLocator Container { get; set; }
        public DotlessConfiguration Config { get; set; }

        private EngineFactory Factory;

        public LessCssHttpHandler()
        {
            Config = new WebConfigConfigurationLoader().GetConfiguration();
            Container = new ContainerFactory().GetContainer(Config);
            Factory = new EngineFactory(Config);
        }

        public void ProcessRequest(HttpContext context)
        {
            var lessFile = Path.Combine(context.Request.PhysicalApplicationPath, context.Request.PhysicalPath);
            var curDir = Path.GetDirectoryName(lessFile);

            var http = new Abstractions.Http();

            var handler = new HandlerImpl(new Abstractions.Http(), new CssResponse(http), Factory.GetEngine(curDir, noCache: true), new FileReader(curDir, noCache: true));
            
            handler.Execute();
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}