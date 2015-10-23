using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SecuruStik.Contorllers
{
    public class PublicKeyUnit:IComparable
    {
        private String _id;
        private SecuruStik.Protocal.PublicKey _pk = null;

        public String ID { get { return this._id; } }
        public SecuruStik.Protocal.PublicKey PK
        {
            get 
            {
                return this._pk;
            }
            set
            {
                this._pk = value; 
            }
        }
        
        public PublicKeyUnit(String id, SecuruStik.Protocal.PublicKey pk)
        {
            this._id = id;
            this.PK = pk;
        }
        public PublicKeyUnit(String id) 
        {
            this._id = id;
        }

        public int CompareTo(object obj)
        {
            PublicKeyUnit pku = obj as PublicKeyUnit;
            return this.ID.CompareTo(pku.ID);
        }
    }

    public class PublicKeyList
    {
        private List<PublicKeyUnit> List = new List<PublicKeyUnit>();

        public PublicKeyUnit this[String ID]
        {
            get
            {
                PublicKeyUnit pku = this.List.Find(_pku => _pku.ID == ID);
                if (pku != null)
                    return pku;
                else
                    return null;
            }
        }

        public Boolean Contains(PublicKeyUnit pku)
        {
            return this.List.Contains(pku);
        }
        public Boolean Contains(String ID)
        {
            return this.List.Exists(pku => pku.ID == ID);
        }
        public void Add(String ID)
        {
            if ( this.Contains(ID) ) return;
            this.List.Add(new PublicKeyUnit(ID));
        }
        public void Add(PublicKeyUnit pku)
        {
            if ( this.Contains(pku) ) return;
            this.List.Add(pku);
        }
        public void Remove(String ID)
        {
            if(this.Contains(ID))
                this.List.Remove(this[ID]);
        }
        public void Remove(PublicKeyUnit pku)
        {
            if (this.Contains(pku))
                this.List.Remove(pku);
        }
    }
}
