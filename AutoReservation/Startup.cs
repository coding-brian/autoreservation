using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Repository;
using Service;
using Service.GenerateMessage;
using Service.MessageFatcory;
using Service.WebAPIRequest;
using Service.WordProcess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutoReservation
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            services.AddHttpClient();
            services.AddScoped<IWebAPIRequest, WebAPIRequest>();
            services.AddScoped<ICoachService, CoachService>();
            services.AddScoped<IMessageFactory, MessageFactory>();
            services.AddScoped<IChangeUserCoachProcess, ChangeUserCoachProcess>();
            services.AddScoped<IWordPrcoessFactory, WordPrcoessFactory>();
            services.AddScoped<IGenerateMessage, GenerateMessage>();

            services.AddTransient<ICoachRepository, CoachRepository>();

            services.AddMvcCore()
        .AddNewtonsoftJson(options =>
           options.SerializerSettings.ContractResolver =
              new CamelCasePropertyNamesContractResolver());

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();

            app.UseSwaggerUI(x =>
            {

                x.SwaggerEndpoint("/swagger/v1/swagger.json", "api v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
