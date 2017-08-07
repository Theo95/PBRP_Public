using GrandTheftMultiplayer.Server.API;
using System.Collections.Generic;
using System.Linq;


namespace PBRP
{
    public class PrisonManager : Script {
        public PrisonManager() { }

        // --- Properties

        public static Dictionary<Player, Prison> activeSentences = new Dictionary<Player, Prison>();

        // --- Methods

        public static void LoadPrisonSentenceForPlayer(Player player) {
            Prison sentence = PrisonRepository.GetPrisonSentenceByCharacterID(player.Id);
            if (sentence == null) { return; }

            activeSentences.Add(player, sentence);
        }

        public static void UnloadPrisonSentenceForPlayer(Player player) {
            if (activeSentences[player] == null) { return; }
            activeSentences.Remove(player);
        }

        public static void TickPrisonSentences() {
            for (int i = 0; i < activeSentences.Count; ++i) {
                Prison sentence = activeSentences.ElementAt(i).Value;
                if (sentence == null) { continue; }
                sentence.Time -= 1;

                if (sentence.Time == 0) {
                    EndSentence(sentence, activeSentences.ElementAt(i).Key);
                }
            }
        }

        public async static void EndSentence(Prison sentence, Player player) {
            if (!sentence.IsPrison) {
                CellManager.StartCellExitSequenceForPlayer(player);
            } else {
                //StartPrisonExitSequenceForPlayer(player);
            }
            await PrisonRepository.RemoveSentence(sentence);
        }
    }
}
