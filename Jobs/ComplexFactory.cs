using System.Threading.Tasks;

namespace Jobs
{
    public class ComplexFactory
    {
        public Task<ComplexObject> CreateSomethingComplicated(string name)
        {
            return Task.FromResult(new ComplexObject()
            {
                Name = name
            });
        }
    }
}
