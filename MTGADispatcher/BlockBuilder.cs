using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MTGADispatcher
{
    //Thoughts: this will likely be a high touch class. Should split this up?
    //Most of this borrowed directly from
    //https://github.com/mtgatracker/mtgatracker/blob/master/app/tasks.py
    public class BlockBuilder : IBlockBuilder
    {
        private readonly List<string> lines =
            new List<string>();

        public void Append(string line)
        {
            lines.Add(line);
        }

        public void Clear()
        {
            lines.Clear();
        }

        public Block? TryBuild()
        {
            //TODO: tons of error handling and missing cases
            if (lines.Count <= 1)
            {
                return null;
            }

            if (lines[1].StartsWith("==>") || lines[1].StartsWith("<=="))
            {
                var blockTitle = lines[1]
                    .Split(new char[] { ' ' }, 2)
                    .Last()
                    .Split('(')
                    .First();

                var json = BuildRequestResponseJson(blockTitle);

                if (json == null)
                {
                    return null;
                }

                return Build(blockTitle, json);
            }

            if (lines[1].Equals("{"))
            {
                var pieces = lines[0].Split(' ');
                var titleBlock = pieces[pieces.Length - 1];
                var json = string.Join("", lines.Skip(1));

                return Build(titleBlock, json);
            }

            return null;
        }

        private Block? Build(string title, string json)
        {
            try
            {
                return new Block(title, JObject.Parse(json));
            }
            catch (Exception)
            {
                //TODO logging
                return null;
            }
        }

        private string? BuildRequestResponseJson(string title)
        {
            if (lines[2].StartsWith("["))
            {
                var result = string.Concat(@"{""", title, @""": ", string.Join("", lines.Skip(2)), @"}");
                return result;
            }
            if (lines[2].StartsWith("{"))
            {
                return string.Join("", lines.Skip(2));
            }
            else
            {
                return null;
            }
        }

        public override string ToString()
        {
            return string.Join("\n", lines);
        }
    }
}
