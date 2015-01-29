
	ArcFurnaceManager arcFurnaceManager;

	void Main(){
		if(arcFurnaceManager == null){
			arcFurnaceManager = new ArcFurnaceManager(GridTerminalSystem);
		}

		arcFurnaceManager.Tick();
	}

public class ArcFurnaceManager : EasyAPI {
	public void SuckInOres() {
	Blocks.NotOfTypeLike("Arc furnace")
		.Items().OfType("Iron").OnlyOres()
		.MoveTo(Blocks.OfTypeLike("Arc furnace"));

	Blocks.NotOfTypeLike("Arc furnace")
		.Items().OfType("Cobalt").OnlyOres()
		.MoveTo(Blocks.OfTypeLike("Arc furnace"));

	Blocks.NotOfTypeLike("Arc furnace")
		.Items().OfType("Nickel").OnlyOres()
		.MoveTo(Blocks.OfTypeLike("Arc furnace"));
	}

	public void EjectIngots() {
	Blocks.OfTypeLike("Arc furnace")
		.Items().OnlyIngots()
		.MoveTo(Blocks.OfTypeLike("Cargo Container"));
	}

	public ArcFurnaceManager(IMyGridTerminalSystem grid) : base(grid) {
	Every(10 * Seconds, Refresh);
	Every(10 * Seconds, SuckInOres);
	Every(10 * Seconds, EjectIngots);
	}
}

/**********************************************************/
/*** EasyAPI class. Extend for easier scripting ***/
/**********************************************************/
public abstract class EasyAPI{
	private int clock;

	private IMyGridTerminalSystem GridTerminalSystem;

	Dictionary<int,List<Action>> Schedule;
	Dictionary<int,List<Action>> Intervals;

	/*** Cache ***/
	public EasyBlocks Blocks;

	/*** Constants ***/
	public const int Seconds = 1;
	public const int Minutes = 60;
	public const int Hours = 3600;
	public const int Days = 86400;

	/*** Constructor ***/
	public EasyAPI(IMyGridTerminalSystem grid){
		this.GridTerminalSystem = grid;
		Schedule = new Dictionary<int,List<Action>>();
		Intervals = new Dictionary<int,List<Action>>();
		this.Reset();
	}

	/*** Execute one tick of the program ***/
	public void Tick(){
		this.clock += 1;

		/*** Handle Intervals ***/
		var e = Intervals.GetEnumerator();
		while(e.MoveNext()){
			var pair = e.Current;
			if(this.clock % pair.Key == 0){
				for(int i = 0; i < pair.Value.Count; i++){
					pair.Value[i]();
				}
			}
		}

		/*** Handle Schedule ***/
		if(Schedule.ContainsKey(this.clock)){
			for(int i = 0; i < Schedule[this.clock].Count; i++){
				Schedule[this.clock][i]();
			}

			Schedule.Remove(this.clock);
		}
	}

	/*** Get the number of ticks since program was compiled ***/
	public int GetClock(){
		return this.clock;
	}

	/*** Call a function at the specified time ***/
	public void At(int time, Action callback){
		if(!Schedule.ContainsKey(time)){
			Schedule.Add(time, new List<Action>());
		}

		Schedule[time].Add(callback);
	}

	/*** Call a function every interval of time ***/
	public void Every(int time, Action callback){
		if(!Intervals.ContainsKey(time)){
			Intervals.Add(time, new List<Action>());
		}

		Intervals[time].Add(callback);
	}

	/*** Call a function in "time" seconds ***/
	public void In(int time, Action callback){
		this.At(this.GetClock() + time, callback);
	}

	/*** Resets the clock and refreshes the blocks.	***/
	public void Reset(){
		this.clock = 0;
		this.Refresh();
	}

	/*** Refreshes blocks.	If you add or remove blocks, call this. ***/
	public void Refresh(){
		Blocks = new EasyBlocks(GridTerminalSystem.Blocks);
	}
}

public class EasyBlocks{
	private List<EasyBlock> Blocks;

	/*** Default, all blocks ***/
	public EasyBlocks(List<IMyTerminalBlock> TBlocks){
		this.Blocks = new List<EasyBlock>();

		for(int i = 0; i < TBlocks.Count; i++){
			EasyBlock Block = new EasyBlock(TBlocks[i]);
			this.Blocks.Add(Block);
		}
	}

	public int Count(){
		return this.Blocks.Count;
	}

	public EasyBlock GetBlock(int i){
		return this.Blocks[i];
	}

	public EasyBlocks(List<EasyBlock> Blocks){
		this.Blocks = Blocks;
	}

	/*** Filters ***/

	public EasyBlocks OfTypeLike(String BlockTypeName){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(this.Blocks[i].Type().Contains(BlockTypeName)){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks NotOfTypeLike(String BlockTypeName){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(!this.Blocks[i].Type().Contains(BlockTypeName)){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks OfType(String BlockTypeName){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(this.Blocks[i].Type() == BlockTypeName){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks NotOfType(String BlockTypeName){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(this.Blocks[i].Type() != BlockTypeName){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks NamedLike(String Name){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(this.Blocks[i].Name().Contains(Name)){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks NotNamedLike(String Name){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(!this.Blocks[i].Name().Contains(Name)){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks Named(String Name){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(this.Blocks[i].Name() == Name){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks NotNamed(String Name){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(this.Blocks[i].Name() != Name){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks NamedRegex(String Pattern){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(this.Blocks[i].NameRegex(Pattern)){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks NotNamedRegex(String Pattern){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		for(int i = 0; i < this.Blocks.Count; i++){
			if(!this.Blocks[i].NameRegex(Pattern)){
				FilteredList.Add(this.Blocks[i]);
			}
		}

		return new EasyBlocks(FilteredList);
	}

	public EasyBlocks First(){
		List<EasyBlock> FilteredList = new List<EasyBlock>();

		if(this.Blocks.Count > 0){
			FilteredList.Add(Blocks[0]);
		}

		return new EasyBlocks(FilteredList);
	}

	/*** Operations ***/

	public EasyBlocks ApplyAction(String Name){
		for(int i = 0; i < this.Blocks.Count; i++){
			this.Blocks[i].ApplyAction(Name);
		}

		return this;
	}

	public EasyBlocks SetProperty<T>(String PropertyId, T value, int bleh = 0){
		for(int i = 0; i < this.Blocks.Count; i++){
			this.Blocks[i].SetProperty<T>(PropertyId, value);
		}

		return this;
	}

	public T GetProperty<T>(String PropertyId, int bleh = 0){
		return this.Blocks[0].GetProperty<T>(PropertyId);
	}

	public EasyBlocks On(){
		for(int i = 0; i < this.Blocks.Count; i++){
			this.Blocks[i].On();
		}

		return this;
	}

	public EasyBlocks Off(){
		for(int i = 0; i < this.Blocks.Count; i++){
			this.Blocks[i].Off();
		}

		return this;
	}

	public EasyBlocks Toggle(){
		for(int i = 0; i < this.Blocks.Count; i++){
			this.Blocks[i].Toggle();
		}

		return this;
	}

	public EasyInventory Items(){
		return new EasyInventory(this.Blocks);
	}

	public void DebugDump(){
		String output = "\n";

		for(int i = 0; i < this.Blocks.Count; i++){
			output += this.Blocks[i].Type() + ": " + this.Blocks[i].Name() + "\n";
		}

		throw new Exception(output);
	}

	public void DebugDumpActions(){
		String output = "\n";

		for(int i = 0; i < this.Blocks.Count; i++){
			output += "[ " + this.Blocks[i].Type() + ": " + this.Blocks[i].Name() + " ]\n";
			output += "*** ACTIONS ***\n";
			List<ITerminalAction> actions = this.Blocks[i].GetActions();

			for(int j = 0; j < actions.Count; j++){
				output += actions[j].Name + "\n";
			}

			output += "\n\n";
		}

		throw new Exception(output);
	}

	public void DebugDumpProperties(){
		String output = "\n";

		for(int i = 0; i < this.Blocks.Count; i++){
			output += "[ " + this.Blocks[i].Type() + ": " + this.Blocks[i].Name() + " ]\n";
			output += "*** PROPERTIES ***\n";
			List<ITerminalProperty> properties = this.Blocks[i].GetProperties();

			for(int j = 0; j < properties.Count; j++){
				output += properties[j].TypeName + ": " + properties[j].Id + "\n";
			}

			output += "\n\n";
		}

		throw new Exception(output);
	}
}

public struct EasyBlock{
	public IMyTerminalBlock Block;

	public EasyBlock(IMyTerminalBlock Block){
		this.Block = Block;
	}

	public String Type(){
		return this.Block.DefinitionDisplayNameText;
	}

	public String Name(){
		return this.Block.CustomName;
	}

	public bool NameRegex(String Pattern, out List<String> Matches){
		System.Text.RegularExpressions.Match m = (new System.Text.RegularExpressions.Regex(Pattern)).Match(this.Block.CustomName);

		Matches = new List<String>();

		bool success = false;
		while(m.Success){
			if(m.Groups.Count > 1){
				Matches.Add(m.Groups[1].Value);
			}
			success = true;

			m = m.NextMatch();
		}

		return success;
	}

	public bool NameRegex(String Pattern){
		List<String> matches;

		return this.NameRegex(Pattern, out matches);
	}

	public ITerminalAction GetAction(String Name){
		return this.Block.GetActionWithName(Name);
	}

	public EasyBlock ApplyAction(String Name){
		ITerminalAction Action = this.GetAction(Name);

		if(Action != null){
			Action.Apply(this.Block);
		}

		return this;
	}

	public T GetProperty<T>(String PropertyId){
		return Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.GetValue<T>(this.Block, PropertyId);
	}

	public EasyBlock SetProperty<T>(String PropertyId, T value){
		try{
			var prop = this.GetProperty<T>(PropertyId);
			Sandbox.ModAPI.Interfaces.TerminalPropertyExtensions.SetValue<T>(this.Block, PropertyId, value);
		}
		catch(Exception e){
		}

		return this;
	}

	public EasyBlock On(){
		this.ApplyAction("OnOff_On");

		return this;
	}

	public EasyBlock Off(){
		this.ApplyAction("OnOff_Off");

		return this;
	}

	public EasyBlock Toggle(){
		if(this.Block.IsWorking){
			this.Off();
		}
		else{
			this.On();
		}

		return this;
	}

	public EasyBlock SetName(String Name){
		this.Block.SetCustomName(Name);

		return this;
	}

	public List<ITerminalAction> GetActions(){
		List<ITerminalAction> actions = new List<ITerminalAction>();
		this.Block.GetActions(actions);
		return actions;
	}

	public List<ITerminalProperty> GetProperties(){
		List<ITerminalProperty> properties = new List<ITerminalProperty>();
		this.Block.GetProperties(properties);
		return properties;
	}

	public EasyInventory Items(Nullable<int> fix_duplicate_name_bug = null){
		List<EasyBlock> Blocks = new List<EasyBlock>();
		Blocks.Add(this);

		return new EasyInventory(Blocks);
	}
}

// Stores all items in matched block inventories for later filtering
public class EasyInventory{
	public List<EasyItem> Items;

	public EasyInventory(List<EasyBlock> Blocks){
		this.Items = new List<EasyItem>();

		// Get contents of all inventories in list and add them to EasyItems list.
		for(int i = 0; i < Blocks.Count; i++){
			EasyBlock Block = Blocks[i];

			for(int j = 0; j < ((IMyInventoryOwner)Block.Block).InventoryCount; j++){
				IMyInventory Inventory = ((IMyInventoryOwner)Block.Block).GetInventory(j);

				List<IMyInventoryItem> Items = Inventory.GetItems();

				for(int k = 0; k < Items.Count; k++){
					this.Items.Add(new EasyItem(Block, j, Inventory, k, Items[k]));
				}
			}
		}
	}

	public EasyInventory(List<EasyItem> Items){
		this.Items = Items;
	}

	public EasyInventory OfType(String SubTypeId){
		List<EasyItem> FilteredItems = new List<EasyItem>();

		for(int i = 0; i < this.Items.Count; i++){
			if(this.Items[i].Type() == SubTypeId){
				FilteredItems.Add(this.Items[i]);
			}
		}

		return new EasyInventory(FilteredItems);
	}

	public EasyInventory OnlyOres(){
		List<EasyItem> FilteredItems = new List<EasyItem>();

		for(int i = 0; i < this.Items.Count; i++){
			if(this.Items[i].IsOre()){
				FilteredItems.Add(this.Items[i]);
			}
		}

		return new EasyInventory(FilteredItems);
	}

	public EasyInventory OnlyIngots(){
		List<EasyItem> FilteredItems = new List<EasyItem>();

		for(int i = 0; i < this.Items.Count; i++){
			if(this.Items[i].IsIngot()){
				FilteredItems.Add(this.Items[i]);
			}
		}

		return new EasyInventory(FilteredItems);
	}

	public EasyInventory InInventory(int Index){
		List<EasyItem> FilteredItems = new List<EasyItem>();

		for(int i = 0; i < this.Items.Count; i++){
			if(this.Items[i].InventoryIndex == Index){
				FilteredItems.Add(this.Items[i]);
			}
		}

		return new EasyInventory(FilteredItems);
	}

	public VRage.MyFixedPoint Count(){
		VRage.MyFixedPoint Total = 0;

		for(int i = 0; i < Items.Count; i++){
			Total += Items[i].Amount();
		}

		return Total;
	}

	public EasyInventory First(){
		List<EasyItem> FilteredItems = new List<EasyItem>();

		if(this.Items.Count > 0){
			FilteredItems.Add(this.Items[0]);
		}

		return new EasyInventory(FilteredItems);
	}

	public void MoveTo(EasyBlocks Blocks, int Inventory = 0){
		for(int i = 0; i < Items.Count; i++){
			Items[i].MoveTo(Blocks, Inventory);
		}
	}
}

// This represents a single stack of items in the inventory
public struct EasyItem{
	private EasyBlock Block;
	public int InventoryIndex;
	private IMyInventory Inventory;
	public int ItemIndex;
	private IMyInventoryItem Item;

	public EasyItem(EasyBlock Block, int InventoryIndex, IMyInventory Inventory, int ItemIndex, IMyInventoryItem Item){
		this.Block = Block;
		this.InventoryIndex = InventoryIndex;
		this.Inventory = Inventory;
		this.ItemIndex = ItemIndex;
		this.Item = Item;
	}

	public String Type(int dummy = 0){
		return this.Item.Content.SubtypeName;
	}

	public bool IsOre() {
		return this.Item.Content is Sandbox.Common.ObjectBuilders.MyObjectBuilder_Ore;
	}

	public bool IsIngot() {
		return this.Item.Content is Sandbox.Common.ObjectBuilders.MyObjectBuilder_Ingot;
	}

	public VRage.MyFixedPoint Amount(){
		return this.Item.Amount;
	}

	public void MoveTo(EasyBlocks Blocks, int Inventory = 0, int dummy = 0){
		// Right now it moves them to all of them.	Todo: determine if the move was successful an exit for if it was.
		// In the future you will be able to sort EasyBlocks and use this to prioritize where the items get moved.
		for(int i = 0; i < Blocks.Count(); i++){
			this.Inventory.TransferItemTo(((IMyInventoryOwner)Blocks.GetBlock(i).Block).GetInventory(Inventory), ItemIndex);
		}
	}
}
