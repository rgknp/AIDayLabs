using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder.BotFramework;
using Microsoft.Bot.Builder.Core.Extensions;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder.LUIS;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PictureBot.Models;
using System;
using System.Text.RegularExpressions;

namespace PictureBot
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(_ => Configuration);
            services.AddBot<PictureBot>(options =>
            {
                options.CredentialProvider = new ConfigurationCredentialProvider(Configuration);
                // Add middleware below
                var middleware = options.Middleware;

                middleware.Add(new UserState<UserData>(new MemoryStorage()));
                middleware.Add(new ConversationState<ConversationData>(new MemoryStorage()));
                // Add Regex ability below
                middleware.Add(new RegExpRecognizerMiddleware()
                    .AddIntent("search", new Regex("search picture(?:s)*(.*)|search pic(?:s)*(.*)", RegexOptions.IgnoreCase))
                    .AddIntent("share", new Regex("share picture(?:s)*(.*)|share pic(?:s)*(.*)", RegexOptions.IgnoreCase))
                    .AddIntent("order", new Regex("order picture(?:s)*(.*)|order print(?:s)*(.*)|order pic(?:s)*(.*)", RegexOptions.IgnoreCase))
                    .AddIntent("help", new Regex("help(.*)", RegexOptions.IgnoreCase)));
                // Add LUIS ability below
                middleware.Add(new LuisRecognizerMiddleware(
    new LuisModel("4791c88c-89b9-4a30-99c8-637c74a0e4bd ", "0db36f78de5f41749a985b66201bebb6", new Uri("https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/"))));

            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseBotFramework();
        }
    }
}