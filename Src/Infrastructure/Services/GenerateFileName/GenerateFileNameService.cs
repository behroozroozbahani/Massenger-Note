using PortalCore.Application.Common.Interfaces;
using System;

namespace PortalCore.Infrastructure.Services.GenerateFileName
{
    public class GenerateFileNameService : IGenerateFileNameService
    {
        public string GenerateFileNameAsync()
        {
            //Random rnd = new Random();
            //int num = rnd.Next(111111,999999);

           return Guid.NewGuid().ToString().Substring(0,6);

            //return DateTime.Now.ToShortDateString().Replace("/", "") + num.ToString();
        }
    }
}