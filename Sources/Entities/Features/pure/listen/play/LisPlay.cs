﻿#if DEBUG
using kifuwarabe_shogithink.pure.control;
using kifuwarabe_shogithink.pure.conv.genkyoku.play;
using kifuwarabe_shogithink.pure.ky;
using kifuwarabe_shogithink.pure.listen.ky;
using kifuwarabe_shogithink.pure.move;
using System.Diagnostics;
using System.Text.RegularExpressions;
using kifuwarabe_shogithink.fen;
#else
using kifuwarabe_shogithink.pure.control;
using kifuwarabe_shogithink.pure.conv.genkyoku.play;
using kifuwarabe_shogithink.pure.ky;
using kifuwarabe_shogithink.pure.listen.ky;
using kifuwarabe_shogithink.pure.move;
using System.Diagnostics;
using System.Text.RegularExpressions;
using kifuwarabe_shogithink.fen;
#endif

namespace kifuwarabe_shogithink.pure.listen.play
{
    public static class LisPlay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="line">B4B3、または toryo といった文字列を含む行☆</param>
        /// <param name="caret">文字カーソルの位置</param>
        /// <param name="ky">取られた駒を調べるために使う☆</param>
        /// <param name="out_move"></param>
        /// <returns></returns>
        public static bool MatchFenMove(
            FenSyurui f,
            string line,
            ref int caret,
            out Move out_move
            )
        {
            if (Util_String.MatchAndNext(Itiran_FenParser.GetToryo(f), line, ref caret))
            {
                out_move = Move.Toryo;
                return true;
            }

            // 「toryo」でも「none」でもなければ、「B4B3」形式と想定して、１手だけ読込み
            // テキスト形式の符号「A4A3 C1C2 …」の最初の１要素を、切り取ってトークンに分解します。

            Match m = Itiran_FenParser.GetMovePattern(f).Match(line, caret);
            if (!m.Success)
            {
                //// 「B4B3」形式ではなかった☆（＾△＾）！？　次の一手が読めない☆
                //string msg = $"指し手のパースに失敗だぜ☆（＾～＾）！ commandline=[{ commandline }] caret=[{ caret }] m.Groups.Count=[{ m.Groups.Count }]";
                //Util_Machine.AppendLine(msg);
                //Util_Machine.Flush();
                //throw new Exception(msg);

                out_move = Move.Toryo;
                return false;
            }

            // m.Groups[1].Value : ABCabc か、 ZKH
            // m.Groups[2].Value : 1234   か、 *
            // m.Groups[3].Value : ABCabc
            // m.Groups[4].Value : 1234
            // m.Groups[5].Value : +

            // カーソルを進めるぜ☆（＾～＾）
            Util_String.SkipMatch(line, ref caret, m);

            // 符号１「B4B3」を元に、move を作ります。
            out_move = TryFenMove2(
                f,
                m.Groups[1].Value,
                m.Groups[2].Value,
                m.Groups[3].Value,
                m.Groups[4].Value,
                m.Groups[5].Value
                );
            Debug.Assert((int)out_move != -1, "");

            return true;
        }
        public static Move TryFenMove2(
            FenSyurui f,
            string str1,
            string str2,
            string str3,
            string str4,
            string str5
            )
        {
            int dstSuji = LisInt.FenSuji_Int(f, str3);// 至筋
            int dstDan = LisInt.FenDan_Int(f, str4);// 至段

            // 取った駒を調べるぜ☆（＾▽＾）
            Masu dstMs = Conv_Masu.ToMasu(dstSuji, dstDan);


            //------------------------------
            // 5
            //------------------------------
            bool natta = false;
            if ("+" == str5)
            {
                // 成りました
                natta = true;
            }


            //------------------------------
            // 結果
            //------------------------------
            if ("*" == str2)
            {
                // 駒台から打ったぜ☆
                return AbstractConvMove.ToMove01cUtta(
                    dstMs,
                    Med_Parser.MojiToMotikomaSyurui(f, str1)//打った駒
                );
            }
            else
            {
                // 盤上の駒を動かしたぜ☆
                if (natta)
                {
                    return AbstractConvMove.ToMove01bNariSasi(Med_Parser.FenSujiDan_Masu(f, str1, str2), dstMs);
                }
                else
                {
                    return AbstractConvMove.ToMove01aNarazuSasi(Med_Parser.FenSujiDan_Masu(f, str1, str2), dstMs);
                }
            }
        }
    }
}
