using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime;
using Amazon.SQS;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MsgProducer
{
    public class Startup
    {
        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder();
            Configuration = builder.Build();

            // var awsOptions = Configuration.GetAWSOptions();
            // awsOptions.Credentials = new EnvironmentVariablesAWSCredentials();
            // services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonSQS>();
            services.Add(new ServiceDescriptor(typeof(IMessageSender), typeof(MessageSender), ServiceLifetime.Singleton));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Run(async (context) =>
            {
                if (context.Request.Query.ContainsKey("msg"))
                {
                    var msg = context.Request.Query["msg"];

                    var sender = app.ApplicationServices.GetService<IMessageSender>();
                    await sender.SendMessage(msg);
                    await context.Response.WriteAsync("Hello World! Sent message: " + msg);
                }
            });
        }
    }
}
