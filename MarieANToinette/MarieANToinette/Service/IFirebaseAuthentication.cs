using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarieANToinette.Service
{
    public interface IFirebaseAuthentication
    {
        Task<string> GetUserId();
    }
}
