using Aiko.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aiko.IServices.IServices;

public interface IServiceBase
{
	AikoAppContext AppContext { get; }
}
