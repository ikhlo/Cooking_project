using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using MySql.Data.MySqlClient;
using System.Threading;
using System.Globalization;
using System.Xml;

namespace Cooking
{
    class Program
    {
        static void Creation_Client(MySqlConnection Co)
        {
            Console.WriteLine("Renseignez les champs suivants :");
            Console.Write("Nom de famille : ");
            string nom = Console.ReadLine();
            string num = "0";
            while (num.Length != 10)
            {
                Console.Write("Numéro de téléphone à 10 chiffres : ");
                num = Console.ReadLine();
            }

            string requete = $"SELECT numeroC from client WHERE numeroC = '{num}';";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader rslt = command.ExecuteReader();
            if (!rslt.HasRows)// si la requete ne renvoie rien c'est que le client n'est pas renseigné
            {
                rslt.Close();
                command.Dispose();
                requete = $"INSERT INTO client(nom,numeroC) VALUES('{nom}','{num}');";
                command = Co.CreateCommand();
                command.CommandText = requete;
                command.ExecuteNonQuery();
                Console.WriteLine("Création du compte client réussie !");
            }
            else
            {
                Console.WriteLine("Compte déjà existant. Veuillez-vous connecter.");
            }
        }

        static Client Identification_Client(MySqlConnection Co)
        {
            Console.WriteLine("Entrez vos identifiants :");
            Console.Write("Nom de famille : ");
            string nom = Console.ReadLine();
            string num = "0";
            while (num.Length != 10)
            {
                Console.Write("Numéro de téléphone à 10 chiffres : ");
                num = Console.ReadLine();
            }

            string requete = $"SELECT nom, numeroC, Id_CdR, solde FROM client where numeroC = '{num}'";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader resultat = command.ExecuteReader();
            if (!resultat.Read())//Base de données renvoie rien
            {
                Console.WriteLine("Aucun compte n'est attribué à ce numéro." +
                "Souhaitez-vous en créer un ?\nTapez 'Y' si oui, et n'importe quelle" +
                "autre touche si non.");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    command.Dispose();
                    requete = $"INSERT INTO client(nom,numeroC) VALUES('{nom}','{num}')";
                    command = Co.CreateCommand();
                    command.CommandText = requete;
                    command.ExecuteNonQuery();
                    Console.WriteLine("\nCréation du compte client réussie ! Veuillez-maintenant vous connecter.");
                    resultat.Close();
                    command.Dispose();
                    return null;
                }
                resultat.Close();
                command.Dispose();
                return null;
            }
            else
            {
                if (resultat.GetString(0) == nom)
                {
                    Console.WriteLine("Authentification réussie !");
                    Client c =  new Client(resultat.GetString(0), resultat.GetString(1), resultat.GetDouble(3));

                    resultat.Close();
                    command.Dispose();

                    requete = $"Select Id_CdR from client where numeroC = '{c.Numero}'";
                    command.CommandText = requete;
                    resultat = command.ExecuteReader();

                    resultat.Read();
                    if (!resultat.IsDBNull(0))
                    {
                        c.Id_CdR = resultat.GetString(0);
                    }
                    resultat.Close();
                    command.Dispose();
                    return c;
                }
                else // nom trouvé mais différent de celui renseigné
                {
                    Console.WriteLine("Le nom écrit ne " +
                        "correspond pas avec le numéro du compte! " +
                        "Veuillez-recommencer : ");
                    int count = 3;
                    while (resultat.GetString(0) != nom && count != 0)
                    {
                        Console.Write("Entrez à nouveau votre nom." +
                          $"Il vous reste {count} tentatives : ");
                        nom = Console.ReadLine();
                        count--;
                    }
                    if (resultat.GetString(0) == nom)
                    {
                        Console.WriteLine("Authentification réussie !");
                        Client c = new Client(resultat.GetString(0), resultat.GetString(1), resultat.GetDouble(3));

                        resultat.Close();
                        command.Dispose();

                        requete = $"Select Id_CdR from client where numeroC = '{c.Numero}'";
                        command.CommandText = requete;
                        resultat = command.ExecuteReader();
                        if (resultat.HasRows)
                        {
                            resultat.Read();
                            c.Id_CdR = resultat.GetString(0);
                        }
                        resultat.Close();
                        command.Dispose();
                        return c;
                    }
                    else
                    {
                        resultat.Close();
                        command.Dispose();
                        Console.WriteLine("Votre compte est bloqué." +
                            "Tentez de vous connecter ultérieurement.");
                        return null;
                    }
                }   
            }
        }

        //Determine la semaine actuelle en fonction d'une date
        static DateTime [] Semaine(int annee, int mois, int jour)
        {
            DateTime Date = new DateTime(annee, mois, jour);
            SortedList<string, int> week = new SortedList<string, int>();
            week.Add("Monday", 0);
            week.Add("Tuesday", 1);
            week.Add("Wednesday", 2);
            week.Add("Thursday", 3);
            week.Add("Friday", 4);
            week.Add("Saturday", 5);
            week.Add("Sunday", 6);

            int numero = week[Convert.ToString(Date.DayOfWeek)];
            DateTime Lundi = Date.AddDays(-numero);
            DateTime Dimanche = Lundi.AddDays(6);
            DateTime[] Limite = { Lundi, Dimanche };
            return Limite;
        }

        static void CdR_Semaine(MySqlConnection Co)
        {
            SortedList<string, int> ID_CDR = new SortedList<string, int>();
            DateTime Actuelle = DateTime.Today;
            DateTime[] Limite = Semaine(Actuelle.Year, Actuelle.Month, Actuelle.Day);
            DateTime Lundi = Limite[0];
            DateTime Dimanche = Limite[1];

            string requete = "SELECT Id_CdR, annee, mois, jour FROM commande natural join recette";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;

            //On ajoute les cdr pour lesquels on des commandes ont été effectuées.
            // L'id Cdr est la clé et la valeur correspond au nombre de commandes de ses plats
            MySqlDataReader resultat = command.ExecuteReader();
            while (resultat.Read())
            {
                DateTime Date = new DateTime(resultat.GetInt32(1), resultat.GetInt32(2), resultat.GetInt32(3));
                if (Date.DayOfYear >= Lundi.DayOfYear && Date.DayOfYear <= Dimanche.DayOfYear && 
                    (Date.Year == Lundi.Year || Date.Year == Dimanche.Year)) 
                {
                    if (ID_CDR.ContainsKey(resultat.GetString(0)))
                    {
                        ID_CDR[resultat.GetString(0)] += 1;
                    }
                    else { ID_CDR.Add(resultat.GetString(0), 1); }
                }
            }
            resultat.Close();
            command.Dispose();

            // On récupère l'ensemble des CdR ayant le plus grand nombre de commande
            if (ID_CDR.Count == 0) { Console.WriteLine("Aucun CdR de la semaine."); }
            else {
                int max = Enumerable.Max(ID_CDR.Values);
                List<string> CdR_semaine = new List<string>();
                foreach (KeyValuePair<string, int> item in ID_CDR)
                {
                    if (item.Value == max) { CdR_semaine.Add(item.Key); }
                }

                foreach (string id in CdR_semaine)
                {
                    requete = $"SELECT nom FROM client WHERE Id_CdR = '{id}'";
                    command.CommandText = requete;
                    resultat = command.ExecuteReader();
                    resultat.Read();
                    Console.Write(resultat.GetString(0) + " ");
                    resultat.Close();
                    command.Dispose();
                }
            }
            Console.WriteLine("");
        }

        static void CdR_OR(MySqlConnection Co)
        {
            string requete = "select Id_CdR, count(*) FROM commande natural join recette group by Id_CdR" +
                " order by count(*) desc;";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader resultat = command.ExecuteReader();
            resultat.Read();
            if (resultat.HasRows)
            {
                string id = resultat.GetString(0);

                resultat.Close();
                command.Dispose();

                requete = $"SELECT nom FROM client WHERE Id_CdR = '{id}'";
                command.CommandText = requete;
                resultat = command.ExecuteReader();
                resultat.Read();
                Console.WriteLine("Le CdR d'Or est " + resultat.GetString(0) + ".");
                resultat.Close();
                command.Dispose();

                requete = $"select nom_recette, type, prix FROM recette WHERE Id_CdR = '{id}' order by count desc";
                command = Co.CreateCommand();
                command.CommandText = requete;

                resultat = command.ExecuteReader();
                int top = 0;
                Console.WriteLine("Ses top recettes sont : \n");
                while (resultat.Read() && top != 5)
                {
                    Console.Write($"\t - {resultat.GetString(0)} ({resultat.GetString(1)}) au prix de" +
                        $" {resultat.GetInt32(2)} cooks\n");
                    top++;
                }
            }
            else
            {
                Console.WriteLine("Aucune commande dans la base de donnée");
            }
            resultat.Close();
            command.Dispose();
        }

        static void Top_Recette(MySqlConnection Co)
        {
            string requete = "select nom_recette, type, prix, count, Id_CdR FROM recette order by count desc";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;

            MySqlDataReader resultat = command.ExecuteReader();
            int top = 0;
            while (resultat.Read() && top != 5)
            {
                Console.Write($"La recette {resultat.GetString(0)} ({resultat.GetString(1)}) crée par {resultat.GetString(4)}" +
                    $" au prix de {resultat.GetInt32(2)} cooks a été commandée {resultat.GetInt32(3)} fois.\n");
                top++;
            }

            resultat.Close();
            command.Dispose();
        }

        static void Reapprovisionnement(MySqlConnection Co)
        {
            Console.WriteLine("L'actualisation des stocks maximums et minimaux va être effectuée.");

            //On récupère d'abord l'ensemble des produits
            string requete = "select nom_produit FROM produit ";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader resultat = command.ExecuteReader();
            List<string> Produit = new List<string>();

            while (resultat.Read())
            {
                Produit.Add(resultat.GetString(0));
            }
            resultat.Close();
            command.Dispose();

            DateTime today = DateTime.Now;
            DateTime limite = today.AddDays(-30);

            // On récupère maintenant l'ensemble des produits pour lesquels il y a au moins une commande passée
            // sur les 30 derniers jours
            requete = "SELECT distinct(nom_produit) FROM composition natural join recette natural join commande " +
                $"WHERE mois >= {limite.Month} and mois <= {today.Month} and annee >= {limite.Year} " +
                $"and annee <= {today.Year}";

            command = Co.CreateCommand();
            command.CommandText = requete;

            resultat = command.ExecuteReader();
            // On va maintenant soustraire ses produits de la liste contenant tous les produits
            // On obtient donc les produits n'ayant pas été utilisés dans les 30 derniers jours.
            while (resultat.Read())
            {
                Produit.Remove(resultat.GetString(0));
            }

            resultat.Close();
            command.Dispose();

            foreach (string produit in Produit)
            {
                requete = "UPDATE produit SET stock_min = stock_min/2, stock_max = " +
                                    $"stock_max/2 WHERE nom_produit = '{produit}'";

                command = Co.CreateCommand();
                command.CommandText = requete;
                command.ExecuteNonQuery();
                Thread.Sleep(1000);

                Console.WriteLine($"{produit} : Stocks actualisés !");
            }

        }

        static void Suppression_R(MySqlConnection Co)
        {
            Console.WriteLine("Choisissez le nom de la recette que vous voulez supprimer.");
            string recette = Console.ReadLine();
            string requete = $"delete FROM composition WHERE nom_recette = '{recette}' ";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            command.ExecuteNonQuery();

            requete = $"delete FROM recette WHERE nom_recette = '{recette}' ";
            command = Co.CreateCommand();
            command.CommandText = requete;
            command.ExecuteNonQuery();

            command.Dispose();
            Console.WriteLine("Suppression effectuée !");
        }

        static void Suppression_C(MySqlConnection Co)
        {
            Console.WriteLine("Indiquez l'id du CdR que vous voulez supprimer. (Les recettes de ce dernier" +
                "seront elles aussi supprimées !)");
            string id = Console.ReadLine();

            string requete = $"delete c FROM composition c join recette r on c.nom_recette = r.nom_recette" +
                $" where Id_CdR = '{id}'; ";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            command.ExecuteNonQuery();
            command.Dispose();

            requete = $"delete FROM recette WHERE Id_CdR = '{id}' ";
            command = Co.CreateCommand();
            command.CommandText = requete;
            command.ExecuteNonQuery();
            command.Dispose();

            //Le créateur de recette reste client
            requete = $"UPDATE client SET Id_CdR = NULL WHERE Id_CdR = '{id}' ";
            command.CommandText = requete;
            command.ExecuteNonQuery();
            command.Dispose();
            Console.WriteLine("Suppression effectuée !");
        }

        static string Nombre_Client(MySqlConnection Co)
        {
            string requete = "SELECT count(*) from client;";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string resultat = $"Il y a {reader.GetString(0)} clients dans la base de donnée Cooking.";
            reader.Close();
            command.Dispose();
            return resultat;
        }

        static string Nombre_CdR(MySqlConnection Co)
        {
            string requete = "SELECT count(*) from client where Id_CdR like '%';";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string resultat = $"Il y a {reader.GetString(0)} créateurs de recette dans la base de donnée Cooking.\n";
            reader.Close();
            command.Dispose();
            requete = "SELECT nom, count(*) from commande natural join recette join client " +
                "on client.Id_CdR = recette.Id_CdR group by client.Id_CdR";
            command.CommandText = requete;
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                resultat += $"{reader.GetString(0)} : {reader.GetString(1)} commandes \n";
            }
            reader.Close();
            command.Dispose();
            return resultat;
        }

        static string Nombre_Recette(MySqlConnection Co)
        {
            string requete = "SELECT count(*) from recette;";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            reader.Read();
            string resultat = $"Il y a {reader.GetString(0)} recettes dans la base de donnée Cooking.\n";
            reader.Close();
            command.Dispose();
            return resultat;
        }

        static string Liste_Produit(MySqlConnection Co)
        {
            string resultat = "";
            string requete = "SELECT nom_produit from produit WHERE stock_actuel <= 2 * stock_min";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            resultat +="Liste des produits ayant une quantité en stock inférieure au double" +
                " de leur quantité minimale : \n\n";
            while (reader.Read())
            {
                resultat += $"{reader.GetString(0)} \n";
            }
            reader.Close();
            command.Dispose();
            return resultat;
        }

        static string Affichage(MySqlConnection Co)
        {
            Console.Write("Veuillez-entrer le nom d'un des produits de la liste précédente pour voir" +
                " les recettes existantes à base de cellui-ci : ");
            string produit;
            string resultat = "";
            produit = Console.ReadLine();
            string requete = $"SELECT nom_recette, quantite, unite from composition natural join produit" +
                $" WHERE nom_produit like '%{produit}%'";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                resultat += $"La recette '{reader.GetString(0)}' necessite {reader.GetString(1)} " +
                    $"{reader.GetString(2)} de {produit}.\n";
            }
            reader.Close();
            command.Dispose();
            return resultat;
        }

        static void Liste_Commande(MySqlConnection Co)
        {
            Console.WriteLine("Réapprovisionnement des stocks en faible quantité.");
            XmlDocument Commande = new XmlDocument();
            XmlElement racine = Commande.CreateElement("Commande");
            Commande.AppendChild(racine);

            XmlDeclaration decl = Commande.CreateXmlDeclaration("1.0", "UTF-8", "no");
            Commande.InsertBefore(decl, racine);
            

            string requete = "SELECT nom_fournisseur, numero_fournisseur from fournisseur;";
            MySqlCommand command = Co.CreateCommand();
            command.CommandText = requete;
            MySqlDataReader reader = command.ExecuteReader();
            SortedList <string,string> Fournisseur = new SortedList<string,string>();

            while (reader.Read())
            {
                Fournisseur.Add(reader.GetString(1), reader.GetString(0));
            }
            reader.Close();
            command.Dispose();

            foreach (KeyValuePair<string, string> item in Fournisseur)
            {
                XmlElement fournisseur = Commande.CreateElement("Fournisseur");
                fournisseur.InnerText = $"{item.Value}";
                racine.AppendChild(fournisseur);

                XmlElement Numero = Commande.CreateElement("Numero_Fournisseur");
                Numero.InnerText = $"{item.Key}";
                fournisseur.AppendChild(Numero);

                requete = "SELECT nom_produit, unite, stock_actuel, stock_max" +
                " from produit natural join fournisseur WHERE stock_actuel <= stock_min" +
                $" AND numero_fournisseur = {Numero.InnerText};";

                command = Co.CreateCommand();
                command.CommandText = requete;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    XmlElement Produit = Commande.CreateElement("Produit");
                    Produit.InnerText = $"{reader.GetString(0)}";
                    fournisseur.AppendChild(Produit);

                    XmlElement quantite = Commande.CreateElement("Quantite_commande");
                    quantite.InnerText = $"{reader.GetDouble(3) - reader.GetDouble(2)}";
                    Produit.AppendChild(quantite);

                    XmlElement unite = Commande.CreateElement("Unite");
                    quantite.InnerText = $"{reader.GetString(1)}";
                    Produit.AppendChild(unite);
                    
                }

                reader.Close();
                command.Dispose();

                requete = "UPDATE produit SET stock_actuel = stock_max" +
                    " WHERE stock_actuel <= stock_min" +
                $" AND numero_fournisseur = {Numero.InnerText};";
                
                command.CommandText = requete;
                command.ExecuteNonQuery();
                command.Dispose();
            }
            DateTime j = DateTime.Today;
            Commande.Save($"Commande-{j.Day}-{j.Month}-{j.Year}.xml");
            Console.WriteLine("Le fichier commande a bien été crée.");
        }

        static void Demo(MySqlConnection Co)
        {

            LinkedList<string> Demo = new LinkedList<string>();
            LinkedListNode<string> Step = Demo.AddLast(Affichage(Co));
            Demo.AddFirst(Liste_Produit(Co));
            Demo.AddFirst(Nombre_Recette(Co));
            Demo.AddFirst(Nombre_CdR(Co));
            Demo.AddFirst(Nombre_Client(Co));

            LinkedListNode<string> current = Demo.First;
            int number = 1;
            while (current != null)
            {
                Console.Clear();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                string numberString = $"- PARTIE {number} -";
                int leadingSpaces = (90 - numberString.Length) / 2;
                Console.WriteLine(numberString.PadLeft(leadingSpaces + numberString.Length));
                Console.WriteLine();

                string content = current.Value;
                Console.WriteLine(content.PadLeft(30));


                Console.WriteLine();
                Console.WriteLine();
                Console.Write(current.Previous != null ? "< PREVIOUS [P]" : GetSpaces(14));
                Console.Write(current.Next != null ? "[N] NEXT >".PadLeft(76) : string.Empty);
                Console.WriteLine();
                Console.WriteLine();

                switch (Console.ReadKey(true).Key)
                {
                    case ConsoleKey.N:
                        if (current.Next != null)
                        {
                            current = current.Next;
                            number++;
                        }
                        break;
                    case ConsoleKey.P:
                        if (current.Previous != null)
                        {
                            current = current.Previous;
                            number--;
                        }
                        break;
                    default:
                        return;
                }
            }

        }

        static void MenuAccueil()
        {
            Console.Clear();
            Console.WriteLine(" Bienvenue sur COOKING ! \n".PadLeft(58));//PadLeft permet d'écrire vers la gauche donc il faut ajouter la partie avant
            Console.WriteLine("Appuyer la touche correspondante :\n".PadLeft(63));
            Console.Write("[C] CLIENT   [G] GESTIONNAIRE   [D] DEMO   [Q] QUITTER\n".PadLeft(73));
        }

        static void MenuClient(MySqlConnection Co)
        {
            do
            {
                Console.Clear();
                Console.WriteLine(" ACCUEIL CLIENT \n".PadLeft(57));
                Console.WriteLine("Appuyer la touche correspondante :\n".PadLeft(63));
                Console.Write("[I] IDENTIFICATION".PadLeft(40));
                Console.Write("[C] CREATION COMPTE\n".PadLeft(35));
                Console.WriteLine();
                switch (Console.ReadKey(true).Key)//Lit la lettre entrée
                {
                    case ConsoleKey.C:
                        {
                            Creation_Client(Co);//Fonction permettant à un utilisateur de créer un compte client
                        }
                        break;
                    case ConsoleKey.I:
                        {
                            Client clt = Identification_Client(Co); //Fonction permettant de créer un objet client et récupérer ses informations lors de son identification
                            Console.Clear();
                            Console.Write("Chargement espace client.");
                            Thread.Sleep(1000);
                            Console.Write(".");
                            Thread.Sleep(1000);
                            Console.Write(".");
                            Thread.Sleep(1000);
                            do
                            {
                                Console.Clear();
                                Console.WriteLine(" ESPACE CLIENT \n".PadLeft(57));
                                Console.WriteLine("Appuyer sur la touche correspondante : \n".PadLeft(63));
                                Console.Write("[P] PASSAGE COMMANDE".PadLeft(28));
                                Console.Write("[S] SOLDE".PadLeft(15));
                                if (clt.Id_CdR != null) //Condition pour savoir si le client est déjà enregistré en tant que CdR
                                {
                                    Console.WriteLine("[E] ESPACE CdR\n".PadLeft(19));//Si oui, on lui affiche la possibilité de rentrer dans l'espace réservé au CdR
                                }
                                else
                                {
                                    Console.WriteLine("[D] DEVENIR CdR\n".PadLeft(19));//Si non, on lui affiche la possibilité de le de devenir
                                }
                                ConsoleKey touche = Console.ReadKey(true).Key;
                                if (touche == ConsoleKey.P)
                                {
                                    clt.Passage_Commande(Co);//Fonction permettant à un client de passer une commande 
                                }
                                if (touche == ConsoleKey.S)
                                {
                                    clt.Consult_Solde(Co);//Fonction permettant à un client de consulter son nombre de cooks (moyen de paiement unique et généralisé dans notre application Cooking
                                }
                                if (touche == ConsoleKey.E)
                                {
                                    MenuCdR(Co, clt);//Fonction permettant au client d'accéder à l'espace réservé au CdR
                                }
                                if (touche == ConsoleKey.D)
                                {
                                    clt.Creation_CdR(Co);//Fonction permettant à un client de devenir CdR
                                }
                                Console.WriteLine("\nAppuyer sur [R] -> Rester dans l'espace client \nAppuyer sur n'importe quelle touche sinon");
                            } while (Console.ReadKey(true).Key == ConsoleKey.R);
                        }
                        break;
                    default: return;
                }
                Console.WriteLine("\nQuitter -> Appuyer sur [Q]\n");
            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
        }

        static void MenuCdR(MySqlConnection Co, Client clt)
        {
            Console.Clear();
            Console.Write("Chargement Espace CdR.");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            Console.Write(".");
            Thread.Sleep(1000);
            do
            {
                Console.Clear();
                Console.WriteLine(" ESPACE CdR \n".PadLeft(50));
                Console.WriteLine("Appuyer sur la touche correspondante : \n".PadLeft(63));
                Console.Write("[C] CREER UNE RECETTE".PadLeft(30));
                Console.Write("[R] AFFICHER MES RECETTES ".PadLeft(30));
                Console.Write("[S] SOLDE\n".PadLeft(20));
                Console.WriteLine();
                switch (Console.ReadKey(true).Key)//Lit la lettre entrée
                {
                    case ConsoleKey.C:
                        {
                            clt.Creation_R(Co);//Fonction permettant au CdR de créer une recette qui lui sera attibuée dans la BDD
                        }
                        break;
                    case ConsoleKey.R:
                        {
                            clt.Affichage_R(Co);//Fonction affichant les recettes du CdR
                        }
                        break;
                    case ConsoleKey.S:
                        {
                            clt.Consult_Solde(Co);//Fonction permettant au CdR de consulter son nombre de cooks 
                        }
                        break;
                    default: return;
                }
                Console.WriteLine("\nAppuyer sur [A] pour rester dans l'espace CdR \nAppuyer sur n'importe quelle touche sinon");
            } while (Console.ReadKey(true).Key == ConsoleKey.A);
        }

        static void MenuGestionnaire(MySqlConnection Co)
        {
            Console.Clear();
            Console.Write("Saisir le mot de passe : ");
            string value = "cooking";
            string mdp = Console.ReadLine();

            if (mdp == value)
            {
                Console.Clear();
                Console.Write("Chargement Espace Gestionnaire.");
                Thread.Sleep(1000);
                Console.Write(".");
                Thread.Sleep(1000);
                Console.Write(".");
                Thread.Sleep(1000);
                do
                {
                    Console.Clear();
                    Console.WriteLine(" ESPACE GESTIONNAIRE \n".PadLeft(55));
                    Console.WriteLine("Appuyer la touche correspondante :\n".PadLeft(63));
                    Console.WriteLine("[C] CUISINIER SEMAINE   [T] TOP RECETTES   [S] SUPPRESSION RECETTE\n".PadLeft(75));
                    Console.WriteLine("[O] CUISINIER D'OR      [D] SUPPRESSION CdR [R] REAPPROVISIONNEMENT\n".PadLeft(75));
                    switch (Console.ReadKey(true).Key)//Lit la lettre entrée
                    {
                        case ConsoleKey.C:
                            {
                                CdR_Semaine(Co);//Fonction permettant d'afficher les meilleurs cuisiniers de la semaine
                            }
                            break;
                        case ConsoleKey.T:
                            {
                                Top_Recette(Co);//Fonction affichant les top recettes
                            }
                            break;
                        case ConsoleKey.S:
                            {
                                Suppression_R(Co);//Fonction permettant au gestionnaire de supprimer une recette 
                            }
                            break;
                        case ConsoleKey.O:
                            {
                                CdR_OR(Co);//Fonction affichant les CdR d'Or
                            }
                            break;
                        case ConsoleKey.D:
                            {
                                Suppression_C(Co);//Fonction permettant au gestionnaire de supprimer un créateur de recette 
                            }
                            break;
                        case ConsoleKey.R:
                            {
                                Liste_Commande(Co);
                                Reapprovisionnement(Co);
                            }                            
                            break;
                        default: return;
                    }
                    Console.WriteLine("\nAppuyer sur [A] pour rester dans l'espace gestionnaire. \nAppuyer sur n'importe quelle touche sinon");
                } while (Console.ReadKey(true).Key == ConsoleKey.A);
            }
            else
            {
                Console.WriteLine("Mot de passe erroné. Retour au menu principal.");
            }
        }

        private static string GetSpaces(int number)
        {
            string result = string.Empty;
            for (int i = 0; i < number; i++)
            {
                result += " ";
            }
            return result;
        }

        static void Main(string[] args)
        {
            MySqlConnection Co = CONNEXION_SQL();
            Co.Open();
            do
            {
                MenuAccueil();//Menu d'accueil de l'application Cooking

                switch (Console.ReadKey(true).Key)//Lit la lettre entrée
                {
                    case ConsoleKey.C:
                        {
                            MenuClient(Co);//Menu permettant à l'utilisateur de s'identifier en tant que client et d'accéder à ses fonctionnalités ou de créer un compte client
                        }
                        break;
                    case ConsoleKey.G:
                        {
                            MenuGestionnaire(Co);//Menu permettant à l'utilisateur d'accéder au gestionnaire de Cooking s'il y est autorisé 
                        }
                        break;
                    case ConsoleKey.D:
                        {
                            Demo(Co); //Demo
                        }
                        break;
                    case ConsoleKey.Q:
                        {
                            //Cas laissant la possibilité à l'utilisateur de quitter l'application dès le début s'il le souhaite
                        }
                        break;
                    default: return;
                }
                Console.WriteLine("\nQuitter et fermer l'application -> Appuyer sur [Q] \nRelancer l'application -> Appuyer sur n'importe quelle touche\n");

            } while (Console.ReadKey(true).Key != ConsoleKey.Q);
            Console.Clear();
            Console.WriteLine("Merci de votre visite et à bientôt sur COOKING !");
            Thread.Sleep(2000);
            Console.Clear();
            Console.WriteLine("Fermeture de l'application en cours");
            Thread.Sleep(2000);
            Co.Close();
        }

        static MySqlConnection CONNEXION_SQL()
        {
            try
            {
                string connexion = "SERVER = localhost; PORT = 3306; " +
                                    "DATABASE=cooking;" +
                                    "UID=root;PASSWORD=*******";

                MySqlConnection Maconnexion = new MySqlConnection(connexion);
                return Maconnexion;
            }
            catch (MySqlException e)
            {
                Console.WriteLine(" ErreurConnexion : " + e.ToString());
                return null;
            }
        }
    }
}
