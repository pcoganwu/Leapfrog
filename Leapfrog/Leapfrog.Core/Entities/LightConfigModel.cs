using Leapfrog.Core.Enums.LightConfigEnums;
using Leapfrog.Core.Enums.RadioConfigEnums;
using System.Collections.ObjectModel;

namespace Leapfrog.Core.Entities
{
    public class LightConfigModel
    {
        public const int NEVER_SET = -1;
        private const int MIN_FLASH_LEN = 4;      // 4 seconds
        private const int MAX_FLASH_LEN = 600;    // 10 min
        private const int MIN_FLASH_DELAY = 0;    // 0 seconds
        private const int MAX_FLASH_DELAY = 600;  // 10 min

        public ObservableCollection<string> DimmerSettings { get; set; } = new ObservableCollection<string>();

        private DimmerSetting _dimmer;
        private readonly DimmerSetting _dimmerDefault = DimmerSetting.LIGHT_SENSOR;
        public DimmerSetting Dimmer
        {
            get => _dimmer;
            set => _dimmer = value;
        }
        public int DimmerIndex
        {
            get => (int)_dimmer;
            set
            {
                if ((DimmerSetting)value == _dimmer) return;

                _dimmer = value < 0 ? DimmerSetting.NO_DIMMING :
                          (DimmerSetting)value >= DimmerSetting.NUM_DIMMER_SETTINGS ? DimmerSetting.ALWAYS_DIMMING :
                          (DimmerSetting)value;
            }
        }
        public string DimmerDevice { get; set; } = NEVER_SET.ToString();

        private string? _flashLen;
        private readonly string _flashLenDefault = "45";
        private int _currentFlashLen;

        public int CurrentFlashLen
        {
            get => _currentFlashLen;
        }

        public string FlashLen
        {
            get => _flashLen ?? string.Empty;
            set
            {
                _flashLen = value;
                FlashLenStatus = int.TryParse(_flashLen, out _currentFlashLen) && _currentFlashLen >= MIN_FLASH_LEN && _currentFlashLen <= MAX_FLASH_LEN
                    ? ConfigStatus.VALID
                    : ConfigStatus.INVALID;
            }
        }
        public ConfigStatus FlashLenStatus { get; set; } = ConfigStatus.VALID;
        public string FlashLenDevice { get; set; } = NEVER_SET.ToString();

        public ObservableCollection<string> LightTypeSettings { get; set; } = new ObservableCollection<string>();
        private LightTypeSetting _lightType;
        public LightTypeSetting LightType
        {
            get => _lightType;
            set => _lightType = value;
        }
        private readonly LightTypeSetting _lightTypeDefault = LightTypeSetting.ENCOM_LIGHTBAR;
        public int LightTypeIndex
        {
            get => (int)_lightType;
            set
            {
                if ((LightTypeSetting)value == _lightType) return;

                _lightType = value < 0 ? LightTypeSetting.DISABLED :
                             (LightTypeSetting)value >= LightTypeSetting.NUM_LIGHT_TYPE_SETTINGS ? LightTypeSetting.ENCOM_LIGHTBAR :
                             (LightTypeSetting)value;
            }
        }
        public string LightTypeDevice { get; set; } = NEVER_SET.ToString();

        public ObservableCollection<string> ButtonTypeSettings { get; set; } = new ObservableCollection<string>();
        private ButtonTypeSetting _buttonType;
        public ButtonTypeSetting ButtonType
        {
            get => _buttonType;
            set => _buttonType = value;
        }
        private readonly ButtonTypeSetting _buttonTypeDefault = ButtonTypeSetting.POLARA_BULLDOG;
        public int ButtonTypeIndex
        {
            get => (int)_buttonType;
            set
            {
                if ((ButtonTypeSetting)value == _buttonType) return;

                _buttonType = value < 0 ? ButtonTypeSetting.DISABLED :
                              (ButtonTypeSetting)value >= ButtonTypeSetting.NUM_BUTTON_TYPE_SETTINGS ? ButtonTypeSetting.CAMPBELL_MOAB :
                              (ButtonTypeSetting)value;
            }
        }
        public string ButtonTypeDevice { get; set; } = NEVER_SET.ToString();

        public ObservableCollection<string> ButtonActionSettings { get; set; } = new ObservableCollection<string>();
        private ButtonActionSetting _buttonAction;
        private readonly ButtonActionSetting _buttonActionDefault = ButtonActionSetting.RESET_TIMER;
        public ButtonActionSetting ButtonAction
        {
            get => _buttonAction;
            set => _buttonAction = value;
        }
        public int ButtonActionIndex
        {
            get => (int)_buttonAction;
            set
            {
                if ((ButtonActionSetting)value == _buttonAction) return;

                _buttonAction = value < 0 ? ButtonActionSetting.DISABLED :
                                (ButtonActionSetting)value >= ButtonActionSetting.NUM_BUTTON_ACTION_SETTINGS ? ButtonActionSetting.RESET_TIMER :
                                (ButtonActionSetting)value;
            }
        }
        public string ButtonActionDevice { get; set; } = NEVER_SET.ToString();

        private string? _flashDelay;
        private readonly string _flashDelayDefault = "0";
        private int _currentFlashDelay;

        public int CurrentFlashDelay
        {
            get => _currentFlashDelay;
        }

        public string FlashDelay
        {
            get => _flashDelay ?? string.Empty;
            set
            {
                _flashDelay = value;
                FlashDelayStatus = int.TryParse(_flashDelay, out _currentFlashDelay) && _currentFlashDelay >= MIN_FLASH_DELAY && _currentFlashDelay <= MAX_FLASH_DELAY
                    ? ConfigStatus.VALID
                    : ConfigStatus.INVALID;
            }
        }
        public ConfigStatus FlashDelayStatus { get; set; } = ConfigStatus.VALID;
        public string FlashDelayDevice { get; set; } = NEVER_SET.ToString();

        public ObservableCollection<string> FlashPatternSettings { get; set; } = new ObservableCollection<string>();
        private FlashPatternSetting _flashPattern;
        public FlashPatternSetting FlashPattern
        {
            get => _flashPattern;
            set => _flashPattern = value;
        }
        private readonly FlashPatternSetting _flashPatternDefault = FlashPatternSetting.MUTCD;
        public int FlashPatternIndex
        {
            get => (int)_flashPattern;
            set
            {
                if ((FlashPatternSetting)value == _flashPattern) return;

                _flashPattern = value < 0 ? FlashPatternSetting.NONE :
                                (FlashPatternSetting)value >= FlashPatternSetting.NUM_FLASH_PATTERN_SETTINGS ? FlashPatternSetting.WIG_WAG :
                                (FlashPatternSetting)value;
            }
        }
        public string FlashPatternDevice { get; set; } = NEVER_SET.ToString();

        public void Init()
        {
            DimmerSettings.Clear();
            DimmerSettings.Add("No Dimming");
            DimmerSettings.Add("Use Light Sensor");
            DimmerSettings.Add("Always Dimming");
            _dimmer = _dimmerDefault;

            FlashLen = _flashLenDefault;

            LightTypeSettings.Clear();
            LightTypeSettings.Add("Disabled");
            LightTypeSettings.Add("Encom Lightbar");
            _lightType = _lightTypeDefault;

            ButtonTypeSettings.Clear();
            ButtonTypeSettings.Add("Disabled");
            ButtonTypeSettings.Add("Campbell 4EVER MOAB");
            ButtonTypeSettings.Add("Polara Bulldog");
            ButtonTypeSettings.Add("Polara Bulldog III iNX");
            _buttonType = _buttonTypeDefault;

            ButtonActionSettings.Clear();
            ButtonActionSettings.Add("Disabled");
            ButtonActionSettings.Add("Reset Flash Timer");
            _buttonAction = _buttonActionDefault;

            FlashDelay = _flashDelayDefault;

            FlashPatternSettings.Clear();
            FlashPatternSettings.Add("None");
            FlashPatternSettings.Add("MUTCD");
            FlashPatternSettings.Add("Wig Wag");
            _flashPattern = _flashPatternDefault;
        }
    }
}
