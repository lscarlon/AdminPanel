using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPanel.Models
{
    public interface IControllerInformationRepository
    {
        List<ControllerInfo> GetAll();
    }
}
