using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using eZdravje.Models;
using Microsoft.Data.SqlClient;

namespace eZdravje.Filters
{
    [AttributeUsage(AttributeTargets.Class  | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAsyncActionFilter
    {
        private const string ApiKeyHeaderName = "ApiKey";
        private const string MailHeaderName = "Mail";
        private const string PasswordHeaderName = "Password";

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string gesloBaza = "";

            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();

            //Mail iz aplikacije
            if(!context.HttpContext.Request.Headers.TryGetValue(MailHeaderName, out var MailVnos))
            {
                context.Result = new UnauthorizedResult();
                
                return;
            }

            //Geslo iz aplikacije
            if(!context.HttpContext.Request.Headers.TryGetValue(PasswordHeaderName, out var GesloVnos))
            {
                context.Result = new UnauthorizedResult();
                
                return;
            }

            using(var sqlConnection1 = new SqlConnection(configuration.GetConnectionString("AzureContext")))
            { 
                using(var cmd = new SqlCommand()
                {
                    CommandText = "SELECT * FROM dbo.AspNetUsers WHERE UserName = @mail",
                    CommandType = CommandType.Text,
                    Connection = sqlConnection1
                })
                {
                    cmd.Parameters.Add("@mail", SqlDbType.Char).Value = MailVnos.ToString();
                    sqlConnection1.Open();

                    using(var reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            gesloBaza = reader[6].ToString();
                        }
                    }
                    sqlConnection1.Close();
                }
            }

            PasswordHasher<string> x = new PasswordHasher<string>();
            var result = x.VerifyHashedPassword("uporabnik",gesloBaza, GesloVnos);

            if(result != PasswordVerificationResult.Success)
            {
                context.Result = new UnauthorizedResult();

                return;
            }

            await next();
        }
    }
}