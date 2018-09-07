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
        public static int MainMenuIndex = 0; // 메인 메뉴 선택 인덱스

        // Note: 밥솥 출력 메소드
        static void RiceCookerBox(int x, int y)
        {
            int height = 20;

            Console.SetCursorPosition(x, y);
            Console.Write("┌──────────────────────┐");
            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│                                            │");
            }

            Console.SetCursorPosition(x, y + height);
            Console.Write("└──────────────────────┘");
        }

        // Note: 쌀통과 물통 출력 메소드
        static void RiceorWaterBox(int x, int y)
        {
            int height = 20;

            Console.SetCursorPosition(x, y);
            Console.Write("┌──────────┐");
            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│                    │");
            }

            Console.SetCursorPosition(x, y + height);
            Console.Write("└──────────┘");
        }

        // Note: 쌀 출력 메소드
        static void RiceHeight(int x, int y, int Amount)
        {
            int Height = Amount / 1000;
            // 지우는 부분
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < 18; i++)
            {
                Console.SetCursorPosition(x, 2 + i);
                Console.Write("                    ");
            }

            for (int i = 0; i < Height; i++)
            {
                Console.SetCursorPosition(x, 19 - i);
                Console.Write("⊙ ⊙ ⊙ ⊙ ⊙ ⊙ ⊙");
            }
        }

        // Note: 물 출력 메소드
        static void WaterHeight(int x, int y, int Amount)
        {
            int Height = Amount / 1000;
            // 지우는 부분
            Console.BackgroundColor = ConsoleColor.Black;
            for (int i = 0; i < 18; i++)
            {
                Console.SetCursorPosition(x, 2 + i);
                Console.Write("                    ");
            }

            Console.BackgroundColor = ConsoleColor.Blue;
            for (int i = 0; i < Height; i++)
            {
                Console.SetCursorPosition(x, 19 - i);
                Console.Write("                    ");
            }

            Console.BackgroundColor = ConsoleColor.Black;
        }

        // Note: 밥솥 상태 정보 박스와 메뉴 박스 출력 메소드
        static void InfoOrMenuBox(int x, int y)
        {
            int height = 13;
            Console.SetCursorPosition(x, y);
            Console.Write("┌──────────────────────┐");
            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│                                            │");
            }

            Console.SetCursorPosition(x, y + height);
            Console.Write("└──────────────────────┘");
        }

        // Note: 뚜껑 열기 닫기 출력 메소드
        static void Cover(bool bOpen)
        {
            const int x = 16;
            if (bOpen)
            {
                Console.SetCursorPosition(x, 3);
                Console.Write("┌┐");
                for (int i = 0; i < 6; i++)
                {
                    Console.SetCursorPosition(x, 4 + i);
                    Console.Write("││");
                }
                Console.SetCursorPosition(x, 10);
                Console.Write("└┘");
            }
            else
            {
                Console.SetCursorPosition(x, 9);
                Console.Write("┌──────────┐");
                Console.SetCursorPosition(x, 10);
                Console.Write("└──────────┘");
            }
        }

        // Note: 밥솥 출력 메소드
        static void RiceBox(int x, int y)
        {
            int height = 7;
            Console.SetCursorPosition(x, y);
            Console.Write("┌──────────┐");
            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│                    │");
            }

            Console.SetCursorPosition(x, y + height);
            Console.Write("└──────────┘");
            Console.SetCursorPosition(x + 10, y + 2);
            Console.Write("밥솥");
            Console.SetCursorPosition(x, y + 6);
            Console.Write("┤"); // 전원 부분
        }

        static void RiceInfo(RiceCookerInfo Info)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.SetCursorPosition(3, 25);
            if (Info.Power)
                Console.Write("전원 상태 : ON");
            else
                Console.Write("전원 상태 : OFF");

            Console.SetCursorPosition(3, 26);
            if (Info.CoverOpenClose)
                Console.Write("뚜껑 상태 : 열림");
            else
                Console.Write("뚜껑 상태 : 닫힘");

            Console.SetCursorPosition(3, 27);
            switch (Info.State)
            {
                case CookerProcess.None:
                    Console.Write("밥솥 상태 : 대기 중  ");
                    break;
                case CookerProcess.Riceing:
                    Console.Write("밥솥 상태 : 밥 넣기  ");
                    break;
                case CookerProcess.Watering:
                    Console.Write("밥솥 상태 : 물 넣기  ");
                    break;
                case CookerProcess.Washing:
                    Console.Write("밥솥 상태 : 쌀 씻기  ");
                    break;
                case CookerProcess.Droping:
                    Console.Write("밥솥 상태 : 물 배수  ");
                    break;
                case CookerProcess.Cooking:
                    Console.Write("밥솥 상태 : 취사 중  ");
                    break;
                case CookerProcess.Completion:
                    Console.Write("밥솥 상태 : 취사 완료  ");
                    break;
                case CookerProcess.Keeping:
                    Console.Write("밥솥 상태 : 보온 중  ");
                    break;
            }

            Console.SetCursorPosition(3, 28);
            Console.Write("인원수 : {0}", Info.Number);
            Console.SetCursorPosition(3, 29);
            Console.Write("쌀 상태 : {0:f1} Kg", Info.Rice / 1000.0f);
            Console.SetCursorPosition(3, 30);
            Console.Write("물 상태 : {0:f1} 리터", Info.Water / 1000.0f);
        }

        static void MessageBox(int x, int y, string Message)
        {
            int height = 3;
            Console.SetCursorPosition(x, y);
            Console.Write("┌───────────────────┐");
            for (int i = 1; i < height; i++)
            {
                Console.SetCursorPosition(x, y + i);
                Console.Write("│                                      │");
            }

            Console.SetCursorPosition(x, y + height);
            Console.Write("└───────────────────┘");
            Console.SetCursorPosition(x + 2, y + 1);
            Console.Write(Message);
        }

        static void OutFrame()
        {
            RiceCookerBox(0, 0);
            RiceorWaterBox(48, 0);
            RiceorWaterBox(72, 0);
            InfoOrMenuBox(0, 21);
            InfoOrMenuBox(48, 21);
            Console.SetCursorPosition(17, 1);
            Console.Write("Smart 밥솥");
            Console.SetCursorPosition(58, 1);
            Console.Write("쌀통");
            Console.SetCursorPosition(82, 1);
            Console.Write("물통");
            Console.SetCursorPosition(18, 23);
            Console.Write("밥솥 정보");
            Console.SetCursorPosition(66, 23);
            Console.Write("(( 메뉴 ))");
        }

        static void Menu(int x, int y, string[] MenuItem)
        {

        }

        static void Main(string[] args)
        {

        }
    }
}
