using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using NpgsqlTypes;

namespace Diebold.TestConsole
{
    class Program
    {
         const string PROD = "server=10.79.15.35;user id=ssvid;password=reKu2E4r;database=frontel";
         const string DEV = "server=10.153.108.196;user id=ssvid;password=W27frAWr;database=frontel";
        static void Main(string[] args)
        {
            Image zdjecie;
            using (NpgsqlConnection conn = new NpgsqlConnection(DEV))
            {

                
                conn.Open();

                NpgsqlTransaction t = conn.BeginTransaction();

                LargeObjectManager lbm = new LargeObjectManager(conn);
                
                LargeObject lo = lbm.Open(1, LargeObjectManager.READWRITE); //take picture oid from metod takeOID

                byte[] buf = new byte[lo.Size()];

                buf = lo.Read(lo.Size());

                MemoryStream ms = new MemoryStream();

                ms.Write(buf, 0, lo.Size());

                lo.Close();

                t.Commit();

                conn.Close();


                 zdjecie = Image.FromStream(ms);
            }

            var temp = 2;
        }
    }
}
