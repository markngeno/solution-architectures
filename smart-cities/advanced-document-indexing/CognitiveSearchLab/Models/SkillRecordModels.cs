using System;
using System.Collections.Generic;
using System.Text;

namespace CustomSkills.Models
{
    public class SkillRequest
    {
        public List<SkillRecord> Values { get; set; }
    }

    public class SkillRecord
    {
        public string RecordId { get; set; }

        public SkillRecordData Data { get; set; }
    }

    public class SkillRecordData
    {
        public string Text { get; set; }

        public string Language { get; set; }
    }
}
