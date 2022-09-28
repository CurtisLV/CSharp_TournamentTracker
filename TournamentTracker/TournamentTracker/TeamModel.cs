﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;

internal class TeamModel
{
    public List<Person> TeamMembers {
        get; set;
    } = new List<Person>();
    public string TeamName {
        get; set;
    }

    public TeamModel()
    {
        TeamMembers = new List<Person>();
    }
}
