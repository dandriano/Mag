
using System;

namespace Mag.Interfaces
{
    public interface IBotInfrastructure 
    {
        public event EventHandler<string> LotReceived;
        public event EventHandler<string> LotCommentReceived;

    }
}
