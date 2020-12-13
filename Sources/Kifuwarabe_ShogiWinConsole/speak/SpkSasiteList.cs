﻿#if DEBUG
using kifuwarabe_shogithink.pure;
using kifuwarabe_shogithink.pure.logger;
using kifuwarabe_shogithink.pure.move;
using kifuwarabe_shogithink.pure.speak.play;
using kifuwarabe_shogithink.pure.control;
#else
using kifuwarabe_shogithink.pure.control;
using kifuwarabe_shogithink.pure.logger;
using kifuwarabe_shogithink.pure.move;
using kifuwarabe_shogithink.pure.speak.play;
#endif

namespace kifuwarabe_shogiwin.speak
{
    public static class SpkSasiteList
    {
        public static void Setumei(FenSyurui f, string header, MoveList sslist, IHyojiMojiretu hyoji)
        {
            hyoji.AppendLine(header);
            hyoji.AppendLine("┌──────────┐");
            for (int i=0; i< sslist.listCount; i++)
            {
                SpkMove.AppendFenTo(f, sslist.moveList[i], hyoji);
                hyoji.AppendLine();
            }
            hyoji.AppendLine("└──────────┘");
#if DEBUG
            hyoji.AppendLine("指し手生成を抜けた場所：" + PureMemory.ssss_sasitePickerWoNuketaBasho1);
#endif
        }
    }
}
