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
    public DeathPanel DeathPanel { get; private set; }
    public AnalysisTargetSelectionPanel AnalysisTargetSelectionPanel { get; private set; }
    public AnalysisResultPanel AnalysisResultPanel { get; private set; }
    public RestPanel RestPanel { get; private set; }
    public SkillLoadoutPanel SkillLoadoutPanel { get; private set; }
    public RelicLoadoutPanel RelicLoadoutPanel { get; private set; }
    public ShopPanel ShopPanel { get; private set; }
    
    // Locations
    public KordorLocation Kordor { get; private set; }
    public WisdomRoomLocation WisdomRoom { get; private set; }
    public WisdomDungeon WisdomDungeon { get; private set; }
    
    public ProgressRoomLocation ProgressRoom { get; private set; }
    public ProgressDungeon ProgressDungeon { get; private set; }
    
    public TeamworkRoomLocation TeamworkRoom { get; private set; }
    public TeamworkDungeon TeamworkDungeon { get; private set; }
    
    public LeadershipRoomLocation LeadershipRoom { get; private set; }
    public LeadershipDungeon LeadershipDungeon { get; private set; }


    public World()
    {
        // 1. Creation Phase
        InventoryPanel = new InventoryPanel();
        StatsPanel = new StatsPanel();
        BattlePanel = new BattlePanel();
        TargetSelectionPanel = new TargetSelectionPanel();
        SkillListPanel = new SkillListPanel();
        BattleEndPanel = new BattleEndPanel();
        DeathPanel = new DeathPanel();
        AnalysisTargetSelectionPanel = new AnalysisTargetSelectionPanel();
        AnalysisResultPanel = new AnalysisResultPanel();
        RestPanel = new RestPanel();
        SkillLoadoutPanel = new SkillLoadoutPanel();
        RelicLoadoutPanel = new RelicLoadoutPanel();
        ShopPanel = new ShopPanel();
        
        Kordor = new KordorLocation(this);
        WisdomRoom = new WisdomRoomLocation(this);
        WisdomDungeon = new WisdomDungeon(this);
        
        ProgressRoom = new ProgressRoomLocation(this);
        ProgressDungeon = new ProgressDungeon(this);
        
        TeamworkRoom = new TeamworkRoomLocation(this);
        TeamworkDungeon = new TeamworkDungeon(this);
        
        LeadershipRoom = new LeadershipRoomLocation(this);
        LeadershipDungeon = new LeadershipDungeon(this);

    }


}
