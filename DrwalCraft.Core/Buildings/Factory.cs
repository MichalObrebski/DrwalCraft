using DrwalCraft.Core;

namespace DrwalCraft.Core.Buildings;

public abstract class Factory : Building, ICanCreate{
    protected bool _inProduction;
    protected SpinLock _productionLock = new();
    public bool InProduction {
        get => _inProduction;
        set{
            _inProduction = value;
            OnPropertyChanged(nameof(InProduction));
        }
    }
    protected int _productionTime;
    protected int _productionProgress;
    public int ProductionTime{
        get => _productionTime;
        protected set{
            _productionTime = value;
            _productionProgress = 0;
        }
    }
    public int ProductionProgress{
        get => _productionProgress;
        set{
            _productionProgress = value;
            OnPropertyChanged(nameof(ProductionProgress));
        }
    }
    protected GameObject? _objectInProduction;
    public GameObject? ObjectInProduction{
        get => _objectInProduction;
        set{
            _objectInProduction = value;
            OnPropertyChanged(nameof(ObjectInProduction));
        }
    }
    public List<ItemToCreate> Products {get; set;}
    public Factory(Player player, string? Icon = null, int size=1) : base(player, Icon, size:size){
        Products = new();
    }

    //pojawia się taki problem (czasami przy pierwszej produkcji ale może też i później(nie zauważyłem tego narazie)) że jednostka natychmiastowo się pojawia bez czekania na zakończenie czasu produkcji
    //produkcja też się wtedy kończy więc problem leży pewnie tam
    //wstawiłem spinlocka i chyba działa
    public virtual void Create(ItemToCreate item){
        if(InProduction) return;

        bool lockTaken = false;
        try{//sekcja krytyczna (create jest wywoływane z innego wątku)
            _productionLock.Enter(ref lockTaken);

            _objectInProduction = item.Make(Owner); //utworzenie instancji produktu
            if(_objectInProduction != null){//jeżeli się udało to ustawienie wartości żeby móc zacząć produkcje
                InProduction = true;
                ProductionTime = item.ProductionTime;
                Owner.Wood -= item.PriceWood;
            }
        }
        finally{
            if(lockTaken)
                _productionLock.Exit();
        }
    }
    /// <summary>
    /// Action made when ProductionProgress gets to 100%
    /// </summary>
    protected abstract void ProductionCompleted();
    /// <summary>
    /// Tick action for producing. 
    /// Assumes that there is something to produce.
    /// </summary>
    protected virtual void Production(){
        bool lockTaken = false;
        //sekcja krytyczna (create jest wywoływane z innego wątku)
        try{
            _productionLock.Enter(ref lockTaken);
            //odliczanie postępu produkcji
            if(_productionProgress >= _productionTime){        
                ProductionCompleted();
            }
            else{
                ProductionProgress++;
            }
        }
        //zwolnienie locka
        finally{
            if(lockTaken)
                _productionLock.Exit();
        }
    }
}