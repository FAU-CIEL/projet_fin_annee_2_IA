using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace projet_fin_annee
{
    public enum Role
    {
        Villageois,
        LoupGarou,
        Voyante,
        Sorcière,
        Chasseur
    }

    public class Joueur
    {
        public string Nom { get; set; }
        public Role Role { get; set; }
        public bool EstVivant { get; set; } = true;

        public Joueur(string nom)
        {
            Nom = nom;
        }
    }

    public class Jeu
    {
        private List<Joueur> joueurs;
        private List<Joueur> eliminationsNuit = new List<Joueur>();

        public Jeu(List<Joueur> joueurs)
        {
            this.joueurs = joueurs;
        }

        public void Jouer()
        {
            while (true)
            {
                Nuit();
                if (ConditionFinPartie()) break;
                Jour();
                if (ConditionFinPartie()) break;
            }
        }

        private bool ConditionFinPartie()
        {
            var vivants = joueurs.Where(j => j.EstVivant).ToList();
            var loupsGarous = vivants.Count(j => j.Role == Role.LoupGarou);
            var autres = vivants.Count(j => j.Role != Role.LoupGarou);

            if (loupsGarous == vivants.Count) // Tous les vivants sont des Loups-Garous
            {
                Console.WriteLine("Les Loups-Garous ont gagné !");
                Thread.Sleep(5000);
                return true;
            }
            else if (autres == vivants.Count) // Tous les vivants sont des rôles autres que Loups-Garous
            {
                Console.WriteLine("Les Villageois ont gagné !");
                Thread.Sleep(5000);
                return true;
            }
            else if (vivants.Count == 0) // Tout le monde est mort
            {
                Console.WriteLine("Tout le monde est mort. La partie est terminée !");
                Thread.Sleep(5000);
                return true;
            }

            return false; // La partie continue
        }

        private void Nuit()
        {
            Console.WriteLine("C'est la nuit.");
            eliminationsNuit.Clear(); // Réinitialiser la liste des éliminations

            // La Voyante choisit un joueur à inspecter
            var voyante = joueurs.FirstOrDefault(j => j.Role == Role.Voyante && j.EstVivant);
            if (voyante != null)
            {
                Console.Write("La Voyante, choisissez un joueur à inspecter : ");
                var nomInspecte = Console.ReadLine();
                var joueurInspecte = joueurs.FirstOrDefault(j => j.Nom.Equals(nomInspecte, StringComparison.OrdinalIgnoreCase));
                if (joueurInspecte != null && joueurInspecte.EstVivant)
                {
                    Console.WriteLine($"{voyante.Nom} a inspecté {joueurInspecte.Nom} et a découvert qu'il est un {joueurInspecte.Role}.");
                }
            }

            // Les Loups-Garous choisissent un joueur à éliminer
            Console.WriteLine("Les Loups-Garous, choisissez un joueur à éliminer : ");
            var nomVictime = Console.ReadLine();
            var victimeNuit = joueurs.FirstOrDefault(j => j.Nom.Equals(nomVictime, StringComparison.OrdinalIgnoreCase) && j.EstVivant);
            if (victimeNuit != null)
            {
                victimeNuit.EstVivant = false;
                eliminationsNuit.Add(victimeNuit); // Ajouter à la liste des éliminations
            }

            // La Sorcière choisit d'utiliser sa potion
            var sorciere = joueurs.FirstOrDefault(j => j.Role == Role.Sorcière && j.EstVivant);
            if (sorciere != null)
            {
                Console.Write("La Sorcière, voulez-vous sauver un joueur ? (oui/non) : ");
                var sauver = Console.ReadLine();
                if (sauver.Equals("oui", StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Choisissez un joueur à sauver : ");
                    var nomSauve = Console.ReadLine();
                    var joueurSauve = joueurs.FirstOrDefault(j => j.Nom.Equals(nomSauve, StringComparison.OrdinalIgnoreCase));
                    if (joueurSauve != null && !joueurSauve.EstVivant)
                    {
                        joueurSauve.EstVivant = true;
                        Console.WriteLine($"{joueurSauve.Nom} a été sauvé par la Sorcière.");
                    }

                    Console.Write("La Sorcière, voulez-vous éliminer un joueur ? (oui/non) : ");
                    var eliminer = Console.ReadLine();
                    if (eliminer.Equals("oui", StringComparison.OrdinalIgnoreCase))
                    {
                        Console.Write("Choisissez un joueur à éliminer : ");
                        var nomElimine = Console.ReadLine();
                        var joueurElimine = joueurs.FirstOrDefault(j => j.Nom.Equals(nomElimine, StringComparison.OrdinalIgnoreCase));
                        if (joueurElimine != null && joueurElimine.EstVivant)
                        {
                            joueurElimine.EstVivant = false;
                            eliminationsNuit.Add(joueurElimine); // Ajouter à la liste des éliminations
                            Console.WriteLine($"{joueurElimine.Nom} a été éliminé par la Sorcière.");
                        }
                    }
                }

                // Annonce des joueurs éliminés à la fin de la nuit
                if (eliminationsNuit.Any())
                {
                    Console.WriteLine("Les éliminations de la nuit :");
                    foreach (var joueur in eliminationsNuit)
                    {
                        Console.WriteLine($"- {joueur.Nom} a été éliminé et était un {joueur.Role}.");
                    }
                }
                else
                {
                    Console.WriteLine("Aucune élimination cette nuit.");
                }
            }
        }
        private void Jour()
        {
            Console.WriteLine("C'est le jour. Les joueurs discutent et votent pour éliminer un joueur.");

            // Afficher les joueurs vivants
            Console.WriteLine("Joueurs vivants :");
            foreach (var joueur in joueurs.Where(j => j.EstVivant))
            {
                Console.WriteLine($"- {joueur.Nom} ({joueur.Role})");
            }

            // Voter pour éliminer un joueur
            Console.Write("Choisissez un joueur à éliminer : ");
            var nomJoueurAEliminer = Console.ReadLine();
            var victimeJour = joueurs.FirstOrDefault(j => j.Nom.Equals(nomJoueurAEliminer, StringComparison.OrdinalIgnoreCase) && j.EstVivant);

            if (victimeJour != null)
            {
                victimeJour.EstVivant = false;
                Console.WriteLine($"{victimeJour.Nom} a été éliminé pendant le jour.");

                // Si le Chasseur est éliminé, il peut éliminer un autre joueur
                if (victimeJour.Role == Role.Chasseur)
                {
                    Console.Write("Le Chasseur peut choisir un joueur à éliminer avec lui : ");
                    var nomCible = Console.ReadLine();
                    var cible = joueurs.FirstOrDefault(j => j.Nom.Equals(nomCible, StringComparison.OrdinalIgnoreCase) && j.EstVivant);
                    if (cible != null)
                    {
                        cible.EstVivant = false;
                        Console.WriteLine($"{cible.Nom} a été éliminé par le Chasseur.");
                    }
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            // Demander le nombre de joueurs
            Console.Write("Combien de joueurs participent ? (minimum 5) : ");
            int nombreDeJoueurs = int.Parse(Console.ReadLine());

            // Vérifier que le nombre de joueurs est valide
            if (nombreDeJoueurs < 5)
            {
                Console.WriteLine("Il doit y avoir au moins 5 joueurs.");
                return;
            }

            var noms = new List<string>();
            for (int i = 1; i <= nombreDeJoueurs; i++)
            {
                Console.Write($"Entrez le pseudo du joueur {i} : ");
                noms.Add(Console.ReadLine());
            }

            // Les rôles à attribuer, en tenant compte du nombre de joueurs
            var roles = new List<Role>
            {
                Role.Villageois,
                Role.LoupGarou,
                Role.Voyante,
                Role.Sorcière,
                Role.Chasseur
            };

            // Ajouter des Villageois supplémentaires si nécessaire
            int villageoisCount = Math.Max(0, nombreDeJoueurs - roles.Count);
            for (int i = 0; i < villageoisCount; i++)
            {
                roles.Add(Role.Villageois);
            }

            // Mélanger les rôles
            Random rand = new Random();
            roles = roles.OrderBy(x => rand.Next()).ToList();

            // Créer les joueurs avec des rôles aléatoires
            var joueurs = new List<Joueur>();
            for (int i = 0; i < noms.Count; i++)
            {
                joueurs.Add(new Joueur(noms[i]) { Role = roles[i] });
            }


            // Initialiser et démarrer le jeu
            Jeu jeu = new Jeu(joueurs);
            jeu.Jouer();
        }
    }
}
