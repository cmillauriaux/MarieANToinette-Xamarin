using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarieANToinette.Service
{
    public interface IFirebaseDatabase
    {
        Task Connect();

        Task SaveData(string dataKind, string[] nodes, Object data);

        Task DeleteData(string dataKind, string[] nodes);

        Task<T> GetData<T>(string dataKind, string[] nodes);
    }
}
