
using Symanto.Jieba.Net.Core.Properties;

namespace Symanto.Jieba.Net.Core
{
    public class ConfigManager
    {
        public static string MainDictFile => Resources.dict;

        public static string ProbTransFile => Resources.prob_trans;

        public static string ProbEmitFile => Resources.prob_emit;

        public static string PosProbStartFile => Resources.pos_prob_start;

        public static string PosProbTransFile => Resources.pos_prob_trans;

        public static string PosProbEmitFile => Resources.pos_prob_trans;

        public static string CharStateTabFile => Resources.char_state_tab;
    }
}