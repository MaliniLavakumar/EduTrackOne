﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EduTrackOne.Domain.Abstractions
{
    interface IDomainEvent
    {
        DateTime OccurredOn { get; }
    }
}
