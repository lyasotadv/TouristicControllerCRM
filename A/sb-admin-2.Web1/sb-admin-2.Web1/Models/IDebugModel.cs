using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sb_admin_2.Web1.Models
{
    public interface IDebugModel
    {
        void CreateTestData();
    }

    abstract public class ModelList<T> : List<T>
        where T : IDebugModel, new()
    {
        public virtual void CreateTestData(int Number)
        {
            for (int n = 0; n<Number; n++)
            {
                T item = new T();
                item.CreateTestData();
                Add(item);
            }
        }
    }
}
