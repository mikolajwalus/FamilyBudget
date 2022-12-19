using Newtonsoft.Json;

namespace FamilyBudget.Server.Infractructure.Middleware
{
    public class BadRequestDetails
    {
        public int StatusCode { get; set; }
        public List<string> Errors { get; set; }
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
