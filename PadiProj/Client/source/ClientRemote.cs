using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PADI_DSTM;

namespace Client
{
    public class ClientRemote : MarshalByRefObject {
        
        public static Form1 form;
        private String ownUrl;
        private List<PadInt> listPadInt = new List<PadInt>();

        delegate void delRSDV(String msg);

        internal void init()
        {
            PADI_DSTM.Library.Init();
        }

        internal String getClientUrl()
        {
            return this.ownUrl;
        }

        internal PadInt getPadInt(int id)
        {
            PadInt pi = null;

            foreach (PadInt p in listPadInt){
                if (p.getId() == id)
                    return p;
            }
            try
            {
                pi = PADI_DSTM.Library.AcessPadInt(id);
                this.setListPadInt(pi);
                return pi;
            }catch(Exception e){
            }
        
            return pi;
        }

        internal void setListPadInt(PadInt p)
        {
            bool ihaveit = false;
            foreach (PadInt item in this.listPadInt)
	        {
                if(item.getId() == p.getId())
                {
                    this.listPadInt.Remove(item);
                    this.listPadInt.Add(p);
                    ihaveit = true;
                    break;
                }
	        }      
            if(!ihaveit)
                this.listPadInt.Add(p); 
        }

        internal List<PadInt> getPadIntList()
        {
            return listPadInt;
        }
    }
}
