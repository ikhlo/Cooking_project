using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cooking
{
    class Recette : IComparable<Recette>
    {
        string nom;
        double prix;
        int count;
        double remuneration;
        string descriptif;
        string id_cdr;

        public Recette(string nom, double prix, int count, double remuneration, string descriptif, string id_cdr)
        {
            this.nom = nom;
            this.prix = prix;
            this.count = count;
            this.remuneration = remuneration;
            this.descriptif = descriptif;
            this.id_cdr = id_cdr;
        }

        public string Nom
        {
            get { return this.nom; }
        }

        public double Prix
        {
            get { return this.prix; }
            set { this.prix = value; }
        }

        public double Remuneration
        {
            get { return this.remuneration; }
            set { this.remuneration = value; }
        }

        public int Count
        {
            get { return this.count; }
            set { this.count = value; }
        }

        public string Descriptif
        {
            get { return this.descriptif; }
        }

        public string Id_Cdr
        {
            get { return this.id_cdr; }
        }

        public int CompareTo(Recette other)
        {
            if (other == null) return 1;
            
            return Nom.CompareTo(other.Nom);
        }
    }
}
