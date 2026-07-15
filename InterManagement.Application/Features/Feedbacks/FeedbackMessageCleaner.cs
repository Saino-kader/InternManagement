using System.Text.RegularExpressions;

namespace InterManagement.Application.Features.Feedbacks
{
    // Retire un préfixe interne du type "[STAGIAIRE:27] " ou "[MENTOR:5] "
    // qui pouvait rester au début du message avant que la création du
    // feedback ne soit corrigée pour ne plus jamais l'ajouter. Les
    // nouveaux messages n'en contiennent plus, mais d'anciennes lignes
    // en base peuvent encore l'avoir — ce nettoyage s'applique à
    // l'affichage, sans modifier les données stockées.
    public static class FeedbackMessageCleaner
    {
        private static readonly Regex LegacyTagPattern =
            new(@"^\[(STAGIAIRE|MENTOR):\d+\]\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static string Clean(string message)
        {
            if (string.IsNullOrEmpty(message))
                return message;

            return LegacyTagPattern.Replace(message, string.Empty);
        }
    }
}
