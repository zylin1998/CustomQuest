using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom
{
    public interface IInitialize
    {
        public IInitialize Initialize();

        public IInitialize Initialize(InitArgs args);
    }

    public interface IInitialize<TInit> where TInit : IInitialize<TInit>
    {
        public TInit Initialize();

        public TInit Initialize(InitArgs args);
    }

    public class InitArgs { }
}
