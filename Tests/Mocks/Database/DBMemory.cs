using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Mocks.Database;
public class DBMemory
{
    private static List<RetailChain> RetailChains { get; set; } =
    [
        new(){ ChainName = "Opti View"},
        new(){ ChainName = "FocalPoint"},
        new(){ ChainName = "Focus Optics"},
        new(){ ChainName = "ClearSight"}
    ];

    public static List<RetailChain> Chains()
    {
        //stores that belong to chains
        RetailChains[0].Stores = [
                    new RetailStore { StoreName = "Opti view Randers", Number = 1, StoreOwner = "Lars Jensen", PostalCode = "8900", Region = "Midtjylland", City = "Randers", Address = "Emmavej 22", Country = "Danmark", Email = "", Phone = "" },
                    new RetailStore { StoreName = "Opti view Aarhus", Number = 2, StoreOwner = "Hans Nielsen", PostalCode = "8000", Region = "Midtjylland", City = "Aarhus", Address = "Åboulevarden 22", Country = "Danmark", Email = "", Phone = "" },
                    new RetailStore { StoreName = "Opti view København", Number = 3, StoreOwner = "Lars Andersen", PostalCode = "2100", Region = "Hovedstaden", City = "København", Address = "Nørrebrogade 22", Country = "Danmark", Email = "", Phone = "" }
                ];

        RetailChains[1].Stores = [
            new RetailStore { StoreName = "FocalPoint Odense", Number = 4, StoreOwner = "Hans Olesen", PostalCode = "5000", Region = "Syddanmark", City = "Odense", Address = "Vestergade 22", Country = "Danmark", Email = "", Phone = "" },
                            new RetailStore { StoreName = "FocalPoint Svendborg", Number = 5, StoreOwner = "Emil Jensen", PostalCode = "5700", Region = "Syddanmark", City = "Svendborg", Address = "Havnegade 22", Country = "Danmark", Email = "", Phone = "" }
        ];

        RetailChains[2].Stores = [
            new RetailStore { StoreName = "Focus Optics Vejle", Number = 6, StoreOwner = "Hans Nielsen", PostalCode = "7100", Region = "Syddanmark", City = "Vejle", Address = "Nørregade 22", Country = "Danmark", Email = "", Phone = "" },
                        ];

        RetailChains[3].Stores = [
        ];

        return RetailChains;
    }
}
