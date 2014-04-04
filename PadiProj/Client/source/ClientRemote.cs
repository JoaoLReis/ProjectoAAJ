﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PADI_DSTM;

namespace Client
{
    public class ClientRemote : MarshalByRefObject{
        
        public static Form1 form;
        private String ownUrl;
        private List<PadInt> listPadInt;

        delegate void delRSDV(String msg);

        internal void init(String url)
        {
            if (url == null)
                throw new System.ArgumentException("URL cannot be null", "url");
            this.ownUrl = url;

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
                else
                    try
                    {
                       pi = PADI_DSTM.Library.AcessPadInt(id);
                       this.setListPadInt(p);
                    }catch(Exception e){
                    }
            }
        }

        internal void setListPadInt(PadInt p)
        {
            this.listPadInt.Add(p);
        }
    }
}
