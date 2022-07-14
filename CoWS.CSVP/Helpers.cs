using CoWS.PayrollVouchers.Models;
using System.Linq;
using System.Security.Principal;
using System.Xml.Serialization;

namespace CoWS.PayrollVouchers
{
    public static class Helpers
    {
        public static string ToXML(this object wsObject)
        {
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(wsObject.GetType());
                serializer.Serialize(stringwriter, wsObject);
                return stringwriter.ToString();
            }
        }

        public static string GetFirstName(this IIdentity identity)
        {
            return new ApplicationDbContext().Users.FirstOrDefault(s => s.UserName == identity.Name).FirstName;
        }

        public static string GetFullName(this IIdentity identity)
        {
            return new ApplicationDbContext().Users.Where(w => w.UserName == identity.Name).Select(s => s.FirstName + " " + s.LastName).FirstOrDefault();
        }
    }
}