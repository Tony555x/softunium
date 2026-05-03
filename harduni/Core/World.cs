using Harduni.Locations;
using Harduni.Panels;

namespace Harduni.Core;

public class World
{
    // Singletons
    public InventoryPanel InventoryPanel { get; private set; }
    public StatsPanel StatsPanel { get; private set; }
    public BattlePanel BattlePanel { get; private set; }
    public TargetSelectionPanel TargetSelectionPanel { get; private set; }
    public SkillListPanel SkillListPanel { get; private set; }
    public BattleEndPanel BattleEndPanel { get; private set; }
    
    // Locations
    public KordorLocation Kordor { get; private set; }
    public WisdomRoomLocation WisdomRoom { get; private set; }
    public WisdomDungeon WisdomDungeon { get; private set; }

    public World()
    {
        // 1. Creation Phase
        InventoryPanel = new InventoryPanel();
        StatsPanel = new StatsPanel();
        BattlePanel = new BattlePanel();
        TargetSelectionPanel = new TargetSelectionPanel();
        SkillListPanel = new SkillListPanel();
        BattleEndPanel = new BattleEndPanel();
        
        Kordor = new KordorLocation(this);
        WisdomRoom = new WisdomRoomLocation(this);
        WisdomDungeon = new WisdomDungeon(this);
    }

    public void Initialize(GameEngine engine)
    {
        // 2. Initialization Phase
        Kordor.InitLinks();
        WisdomRoom.InitLinks();
        WisdomDungeon.InitLinks();
    }
}
