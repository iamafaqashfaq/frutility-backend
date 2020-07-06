using frutility_backend.Data.Model;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace frutility_backend.Helpers
{
    public class DecodeToken
    {
        public DecodeToken()
        {

        }
        public Claim TokenDecoder(string entoken)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokens = handler.ReadToken(entoken) as JwtSecurityToken;
            var decoded = tokens.Claims.FirstOrDefault(t => t.Type == "unique_name");
            return decoded;
        }
    }
}
