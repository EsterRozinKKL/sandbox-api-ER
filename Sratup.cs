using Microsoft.OpenApi.Models;
using System.Data;
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using sandboxEr.Repositories.Implementations;
using sandboxEr.Repositories.Interfaces;
namespace sandboxEr
{       
      public class Startup
    {
 public Startup()
        {

        }

      






        // This method gets called by the runtime. Use this method to add services to the container.

     public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policyBuilder => policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
                      
            });
            // configure HttpClient for calling other APIs 
            services.AddHttpClient();
            services.AddHttpContextAccessor();
            services.AddScoped<IGetToken,GetToken>();
               services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("dropbox", new OpenApiInfo { Title = "dropbox" });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors("CorsPolicy");
            
            if (env.IsDevelopment())
            {

                app.UseDeveloperExceptionPage();
        
               }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger(c =>
                {
                    c.RouteTemplate = "/{documentName}/openapi.json";
                });
                app.UseSwaggerUI(c => {c.RoutePrefix = string.Empty;c.SwaggerEndpoint("/dropbox/openapi.json", "dropbox");});
          
        
        
             }


    }
}
