using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using System.Globalization;

namespace Cooking
{
    class Client
    {

        // attributs
        string nom;
        string numero;
        string id_CdR;
        double solde;

        // def constructeur
        public Client(string nom, string numero, double solde = 0, string id_CdR = null)
        {
            this.nom = nom;
            this.numero = numero;
            this.solde = solde;
            this.id_CdR = id_CdR;
        }

        //def proprietes
        public string Nom
        {
            get { return this.nom; }
        }

        public string Numero
        {
            get { return this.numero; }
            set { this.numero = value; }
        }

        public string Id_CdR
        {
            get { return this.id_CdR; }
            set {  this.id_CdR = value;  }
        }

        public double Solde
        {
            get { return this.solde; }
            set { this.solde = value; }
        }

        public void Passage_Commande(MySqlConnection Co)
        {
            Console.Clear();
            // recette avec une quantité d'ingrédients nécessaire
            string requete = "select nom_recette, prix, count, rémunération, descriptif, Id_CdR from recette ";

            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader resultat = command.ExecuteReader();
            int count = 1;
            List<Recette> R = new List<Recette>();
            SortedList<Recette, int> Choix = new SortedList<Recette, int>();
            while (resultat.Read())
            {
                Recette Recette = new Recette(resultat.GetString(0), resultat.GetFloat(1), resultat.GetInt32(2),
                    resultat.GetFloat(3), resultat.GetString(4), resultat.GetString(5));
                R.Add(Recette);
                Console.WriteLine("Numéro - " + count);
                Console.WriteLine("Nom : " + resultat.GetString(0));
                Console.WriteLine("Descriptif : " + resultat.GetString(4));
                Console.WriteLine("Prix : " + resultat.GetString(1));
                Console.WriteLine("\n");
                count++;
            }
            resultat.Close();
            command.Dispose();

            Console.Write("Combien de plat souhaitez-vous commander ? ");
            int nombre_plat = Convert.ToInt32(Console.ReadLine());
            while (nombre_plat > 0)
            {
                Console.WriteLine("\n Quel plat souhaitez-vous commander ? Entrez le numéro du plat choisis.");
                int numéro = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("\n Combien en voulez-vous ?");
                int nombre = Convert.ToInt32(Console.ReadLine());
                Choix.Add(R[numéro-1],nombre);
                nombre_plat -= nombre;
            }

            Console.Clear();
            Console.WriteLine("\nVoici un résumé de votre commande.\n");
            double total = 0;
            foreach (KeyValuePair<Recette, int> item in Choix)
            {
                Console.Write($"{item.Value} * {item.Key.Nom} \nPrix : {item.Value*item.Key.Prix} cooks\n");
                total += item.Value * item.Key.Prix;
            }

            Console.Write("\n Soit un montant total de " + total +
                "\n Souhaitez-vous valider votre commande? Appuyez sur 'Y' si oui et une autre touche si non. ");

            if (Console.ReadKey().Key == ConsoleKey.Y && this.Solde >= total)
            {

                int nombre = 0;

                foreach (KeyValuePair<Recette, int> item in Choix)
                {
                    resultat.Close();
                    command.Dispose();

                    List<string> produit = new List<string>();
                    List<double> quantite = new List<double>();
                    requete = "SELECT nom_produit, quantite FROM composition WHERE" +
                        $" nom_recette = '{item.Key.Nom}'";
                    command.CommandText = requete;
                    resultat = command.ExecuteReader();
                    while (resultat.Read())
                    {
                        produit.Add(resultat.GetString(0));
                        quantite.Add(resultat.GetDouble(1));
                    }
                    resultat.Close();
                    command.Dispose();

                    //Verification qu'on possède les quantités necessaires
                    bool verif = true;
                    for (int i = 0; i < produit.Count; i++)
                    {
                        requete = "SELECT stock_actuel from produit " +
                                $"WHERE nom_produit = '{produit[i]}'";
                        command.CommandText = requete;
                        resultat = command.ExecuteReader();
                        resultat.Read();
                        if (resultat.GetDouble(0) < item.Value * quantite[i])
                        {
                            verif = false;
                            break;
                        }
                        resultat.Close();
                        command.Dispose();
                    }

                    //Si on a ce qu'il faut
                    if (verif == true)
                    {
                        // Actualisation du nombre d'ingrédients
                        for (int j = 0; j < produit.Count; j++)
                        {
                            NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                            nfi.NumberDecimalSeparator = ".";
                            double usage = item.Value * quantite[j];
                            requete = "UPDATE produit SET stock_actuel = stock_actuel - " + 
                                usage.ToString(nfi)+
                                $" WHERE nom_produit = '{produit[j]}'";
                            command.CommandText = requete;
                            command.ExecuteNonQuery();
                        }

                        // On récupère l'index de la recette et on va la chercher dans la liste des recettes
                        int compteur = R[R.IndexOf(item.Key)].Count;
                        R[R.IndexOf(item.Key)].Count += item.Value;

                        // Rémunération du créateur de recette
                        requete = $"UPDATE client SET solde = solde + {item.Value * item.Key.Remuneration}" +
                            $" WHERE Id_CdR = '{item.Key.Id_Cdr}'";
                        command.CommandText = requete;
                        command.ExecuteNonQuery();
                        command.Dispose();

                        //Création d'une commande dans la table commande
                        int n = 0;
                        while (n < item.Value)
                        {
                            string heure = DateTime.Now.Hour.ToString("D2");
                            string minute = DateTime.Now.Minute.ToString("D2");
                            string annee = DateTime.Now.Year.ToString("D4");
                            string mois = DateTime.Now.Month.ToString("D2");
                            string jour = DateTime.Now.Day.ToString("D2");
                            string id_commande = $"C{heure}H{minute}{this.Nom[0]}{nombre}";

                            requete = $"INSERT INTO commande(id_Commande,numeroC,nom_recette,annee,mois,jour" +
                                $") VALUES('{id_commande}','{this.Numero}','{item.Key.Nom}'," +
                                $"'{annee}','{mois}','{jour}')";
                            command.CommandText = requete;
                            command.ExecuteNonQuery();
                            command.Dispose();
                            n++;
                            nombre++;

                        }
                        
                        foreach (Recette r in R)
                        {
                            if (Choix.ContainsKey(r)) // Si la recette a été commandée
                            {
                                requete = $"UPDATE recette SET count = {r.Count} WHERE nom_recette = '{r.Nom}'";
                                command.CommandText = requete;
                                command.ExecuteNonQuery();
                                command.Dispose();

                                // On vérifie que le compteur VIENT dépasser 10
                                if (r.Count > 9 && r.Count < 50 && compteur < 10)
                                {
                                    r.Prix += 2;
                                    requete = $"UPDATE recette SET prix = {r.Prix}" +
                                        $" WHERE nom_recette = '{r.Nom}'";
                                    command.CommandText = requete;
                                    command.ExecuteNonQuery();
                                    command.Dispose();
                                }

                                
                                if (r.Count > 50 && compteur < 50)
                                {
                                    r.Prix += 5; r.Remuneration = 4;
                                    requete = $"UPDATE recette SET prix = {r.Prix}, rémunération = " +
                                        $"{r.Remuneration} WHERE nom_recette = '{r.Nom}'";
                                    command.CommandText = requete;
                                    command.ExecuteNonQuery();
                                    command.Dispose();
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nNous ne disposons pas assez d'ingrédients pour {item.Value} " +
                            $"{item.Key.Nom}.\n Veuillez-réessayer plus tard ou avec une plus faible quantité.");
                        total -= item.Value * item.Key.Prix;
                    }
                }

                resultat.Close();
                command.Dispose();
                requete = $"SELECT solde from client WHERE numeroC = '{this.Numero}'";
                command.CommandText = requete;
                resultat = command.ExecuteReader();
                resultat.Read();
                this.Solde = resultat.GetDouble(0);
                resultat.Close();
                command.Dispose();

                this.Solde -= total;
                Console.WriteLine("\n Votre commande a bien été traitée. Voici votre solde restant "
                    + this.Solde + "\n Votre commande est actuellement en cours de préparation.\n" +
                    " MERCI A VOUS ET A BIENTOT");

                requete = $"UPDATE client SET solde = solde - {total} " +
                        $" WHERE numeroC = '{this.Numero}'";
                command.CommandText = requete;
                command.ExecuteNonQuery();
                command.Dispose();
            }
            else
            {
                Console.WriteLine("\nPaiement refusé. Vore solde ne contient pas assez de cooks." +
                    "\nNous vous invitons à recharger depuis le menu principal.");
            }
        }


        public void Creation_CdR(MySqlConnection Co)
        {
            Console.Write("Entrez l'identifiant CdR souhaité : ");
            string id = Console.ReadLine();
            string requete = $"SELECT * FROM client WHERE Id_CdR = '{id}';";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader rslt = command.ExecuteReader();
            if (!rslt.HasRows)
            {
                rslt.Close();
                command.Dispose();
                requete = $"UPDATE client SET Id_CdR = '{id}' where numeroC = '{this.numero}';";
                command = Co.CreateCommand();
                command.CommandText = requete;
                command.ExecuteNonQuery();
                Console.WriteLine("Création du client en tant que CdR réussie !");
                Console.WriteLine("Votre espace CdR sera accessible lors de votre prochaine connexion.");
                command.Dispose();
            }
            else
            {
                Console.WriteLine("L'identifiant CdR que vous avez choisi est déjà pris.");
                rslt.Close();
                command.Dispose();
            }
        }

        public void Affichage_R(MySqlConnection Co)
        {
            string requete = $"SELECT nom_recette, count FROM recette WHERE id_CdR = '{this.id_CdR}' ";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader resultat = command.ExecuteReader();
            if (resultat.HasRows)
            {
                while (resultat.Read())
                {
                    Console.WriteLine($"Recette : {resultat.GetString(0)}, commandée {resultat.GetInt32(1)} fois.");
                }
            }
            else { Console.WriteLine("Vous n'avez pas encore créer de recettes."); }

            resultat.Close();
            command.Dispose();
        }

        public void Consult_Solde(MySqlConnection Co)
        {
            string requete = $"SELECT solde FROM client WHERE numeroC = '{this.numero}' ";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader resultat = command.ExecuteReader();
            while (resultat.Read())
            {
                Console.WriteLine($"Votre solde est de {resultat.GetDouble(0)} cooks.");
            }
            resultat.Close();
            command.Dispose();
        }

        public void Creation_R(MySqlConnection Co)
        {
            string nom = "";
            string type = "";
            string descriptif = "";
            int prix = 0;
            Console.WriteLine("Quel est le nom de votre recette ?");
            nom = Console.ReadLine();
            Console.WriteLine("Saisir entree, plat ou dessert :");
            type = Console.ReadLine();
            while (prix < 10 || prix > 40)
            {
                Console.WriteLine("Saisir un prix entre 10 et 40 cooks");
                prix = Convert.ToInt32(Console.ReadLine());
            }
            while (descriptif.Length >= 256 || descriptif.Length == 0)
            {
                Console.WriteLine("Decrivez votre recette en 256 caractères maximum sans apostrophes.");
                descriptif = Console.ReadLine();
            }
            string requete = $"INSERT INTO `cooking`.`recette`(`nom_recette`,`type`,`prix`,`count`,`rémunération`,`descriptif`,`Id_CdR`) " +
                $"VALUES('{nom}','{type}',{prix},0,2,'{descriptif}','{this.id_CdR}');";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            command.ExecuteNonQuery();
            command.Dispose();
            Console.WriteLine($"Votre recette {nom} a bien été créé.");
            List<string> produit = new List<string>();
            List<string> unite = new List<string>();
            requete = $"SELECT nom_produit, unite FROM produit ";
            command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader resultat = command.ExecuteReader();
            Console.WriteLine("Vous allez maintenant choisir les ingrédients qui composeront votre recette.");
            Console.WriteLine("Voici la liste de nos produits :");
            int count = 1;
            while (resultat.Read())
            {
                produit.Add(resultat.GetString(0));
                unite.Add(resultat.GetString(1));
                Console.WriteLine($"{count} - {resultat.GetString(0)} en {resultat.GetString(1)} ");
                count++;
            }
            resultat.Close();
            command.Dispose();
            do
            {
                Console.WriteLine("\nSaisir le numéro de l'ingrédient correspondant :");
                int index = Convert.ToInt32(Console.ReadLine());
                Console.WriteLine("\nSaisir la quantité :");
                double quantite = Convert.ToDouble(Console.ReadLine());
                requete = "INSERT INTO `cooking`.`composition`(`nom_recette`,`nom_produit`,`quantite`)" +
                           $"VALUES('{nom}', '{produit[index - 1]}', {quantite}); ";
                command = Co.CreateCommand();
                command.CommandText = requete;
                command.ExecuteNonQuery();
                command.Dispose();
                Console.WriteLine("Appuyez sur [T] pour terminer ou n'importe quelle autre touche pour ajouter un nouvel ingredient.");
            } while (Console.ReadKey(true).Key != ConsoleKey.T);
            Console.WriteLine("La composition de votre recette a bien été enregistré.");
        }
    }
}

