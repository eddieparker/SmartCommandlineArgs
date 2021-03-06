﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCmdArgs.ViewModel;

namespace SmartCmdArgs.Logic
{
    class ToolWindowDataSerializer
    {
        protected static List<ListEntryData> TransformCmdList(ICollection<CmdBase> items)
        {
            var result = new List<ListEntryData>(items.Count);
            foreach (var item in items)
            {
                var newElement = new ListEntryData
                {
                    Id = item.Id,
                    Command = item.Value
                };
                
                if (item is CmdContainer container)
                    newElement.Items = TransformCmdList(container.Items);

                result.Add(newElement);
            }
            return result;
        }
    }
}
