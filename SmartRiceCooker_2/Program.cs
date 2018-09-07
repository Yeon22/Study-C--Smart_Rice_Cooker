using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Media;

/* Note: 개선할 사항
    1. 예약에 대한 시간 처리 
       예약을 하기 위해서는 쓰레드가 필요하며 현재는 기본 문법을 사용하여 구현하는 것을 
       목표로 하므로 제외됨
    2. 쌀통과 물통을 분리한 객체로 구현하는 것이 현실성 있는 형태
       밥솥, 쌀통, 물통의 객체로 구현 
       현재는 구조체에 다 통합되어 있다.
    3. 물통을 정수기로 업그레이드하고 정수기에서 물을 자체적으로 공급하는 형태로 구현
    4. 현재는 과정을 보기 위해 3초 단위로 과정을 진행시켰지만 관리자 프로그램으로 
       과정에 대한 시간 등 에 대한 정보를 저장하고 밥솥은 데이터 파일을 읽어 과정을 진행하도록 한다면
       더 이상적일 것이다. 그러면 다양한 밥맛을 필요에 따라 관리자 프로그램에서 만들고
       밥솥에서는 이 정보를 읽어 밥을 할 수 있을 것이다. ^^

  [참고사항]
    쌀 일인분 160g, 물의 양은 대략 170ml 정도 (정확지는 않음) 
    1 kg은 1000 g, 1 리터는 1000 ml
*/

namespace SmartRiceCooker_2
{
    enum CookerProcess { None, Riceing, Watering, Washing, Droping, Cooking, Completion, Keeping };
    
    struct RiceCookerInfo
    {
        public bool CoverOpenClose; // 뚜껑 열기 닫기
        public bool Power; // 전원 On/Off
        public CookerProcess State; // 밥솥 진행 상태
        public int Rice; // 쌀의 양 g(그램) 기준, 출력 때 환산
        public int Water; // 물의 양 ml(미리리터) 기준. 출력 때 환산
        public int Number; // 인원수

        public RiceCookerInfo(int _Rice, int _Water)
        {
            Rice = _Rice;
            Water = _Water;
            State = CookerProcess.None;
            CoverOpenClose = false;
            Power = false;
            Number = 0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {

        }
    }
}
