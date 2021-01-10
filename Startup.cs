using ApiCatalogo.Context;
using ApiCatalogo.Mappings;
using ApiCatalogo.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ApiCatalogo
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
            services.AddDbContext<CatalogoDBContext>(options =>
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection")));

            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<CatalogoDBContext>()
                .AddDefaultTokenProviders();

            //JWT
            //adiciona o manipulador de autenticacao e define o esquema de autenticacao usado: bearer
            //valida o emissor, a audiencia e a chave
            //e usando a chave secreta privada valida a assinatura
            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidAudience = Configuration["TokenConfiguration:Audience"],
                    ValidIssuer = Configuration["TokenConfiguration:Issuer"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                });

            //Registrar o gerador swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CatalogoAPI",
                    Description = "Catálogo de Produtos e Categorias",
                    TermsOfService = new Uri("https://www.irineusjr.com.br/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Irineu",
                        Email = "irineusjr@yahoo.com.br",
                        Url = new Uri("https://www.irineusjr.com.br"),
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Usar sobre LICX",
                        Url = new Uri("https://www.irineusjr.com.br/license"),
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddControllers()
                    .AddNewtonsoftJson(options => 
                    options.SerializerSettings.ReferenceLoopHandling = 
                    Newtonsoft.Json.ReferenceLoopHandling.Ignore);
            services.AddScoped<IUnitOfWork, UnitOfWork>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //the default HSTS value is 30 days.
                //you may want to change this for production scenarios, see
                //https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //adiciona o middleware para redirecionar para https
            app.UseHttpsRedirection();

            //adiciona o middleware de roteamento
            app.UseRouting();

            //adiciona o middleware que habilita autenticacao
            app.UseAuthentication();

            //adiciona o middleware que habilita autorização
            app.UseAuthorization();

            app.UseCors(opt => opt.AllowAnyOrigin());

            //swagger
            app.UseSwagger();

            //swaggerUI
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json",
                    "Catálogo de Produtos e Categorias");
            });
            
            //adiciona o middleware que executa o endpoint do request atual
            app.UseEndpoints(endpoints =>
            {
                //adiciona os endpoints para as actions dos controladores sem especificar rotas
                endpoints.MapControllers();
            });
        }
    }
}
