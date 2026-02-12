using System;
using System.IO;
using System.Text;

// ========================================
// 設備日常点検記録アプリ
// ========================================
// このアプリは工場や施設の設備点検を記録するためのシンプルなコンソールアプリです。
// 点検結果（OK/NG）とコメントを記録し、CSV形式で保存します。

namespace InspectionApp
{
    class Program
    {
        // 点検対象の設備リスト
        // 新しい設備を追加する場合は、このリストに追加してください
        private static readonly string[] EquipmentList = new string[]
        {
            "ポンプA",
            "ポンプB",
            "コンプレッサー",
            "ボイラー",
            "冷却塔",
            "電気盤",
            "配管システム",
            "換気扇"
        };

        static void Main(string[] args)
        {
            Console.WriteLine("========================================");
            Console.WriteLine("   設備日常点検記録アプリ");
            Console.WriteLine("========================================");
            Console.WriteLine();

            // 継続して点検を記録できるようにループ
            bool continueInspection = true;
            while (continueInspection)
            {
                // 点検情報を入力
                string equipmentName = SelectEquipment();
                string result = InputInspectionResult();
                string comment = InputComment();
                
                // 現在の日時を取得（yyyy-MM-dd HH:mm形式）
                string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                
                // CSV形式でファイルに保存
                SaveToCSV(dateTime, equipmentName, result, comment);
                
                Console.WriteLine();
                Console.WriteLine("✓ 点検記録を保存しました。");
                Console.WriteLine();
                
                // 続けるかどうかを確認
                Console.Write("続けて点検を記録しますか？ (y/n): ");
                string? response = Console.ReadLine()?.Trim().ToLower();
                continueInspection = (response == "y" || response == "yes");
                Console.WriteLine();
            }
            
            Console.WriteLine("アプリを終了します。お疲れ様でした。");
        }

        /// <summary>
        /// 設備を選択または入力する
        /// </summary>
        /// <returns>選択または入力された設備名</returns>
        private static string SelectEquipment()
        {
            Console.WriteLine("--- 設備選択 ---");
            Console.WriteLine("点検する設備を選択してください：");
            
            // 設備リストを表示
            for (int i = 0; i < EquipmentList.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {EquipmentList[i]}");
            }
            Console.WriteLine($"  {EquipmentList.Length + 1}. その他（手入力）");
            Console.WriteLine();
            
            // ユーザーの選択を取得
            while (true)
            {
                Console.Write("番号を入力してください: ");
                string? input = Console.ReadLine();
                
                if (int.TryParse(input, out int choice))
                {
                    // リストから選択
                    if (choice >= 1 && choice <= EquipmentList.Length)
                    {
                        string selected = EquipmentList[choice - 1];
                        Console.WriteLine($"選択: {selected}");
                        Console.WriteLine();
                        return selected;
                    }
                    // 手入力を選択
                    else if (choice == EquipmentList.Length + 1)
                    {
                        Console.Write("設備名を入力してください: ");
                        string? customName = Console.ReadLine()?.Trim();
                        if (!string.IsNullOrEmpty(customName))
                        {
                            Console.WriteLine();
                            return customName;
                        }
                        Console.WriteLine("設備名を入力してください。");
                    }
                    else
                    {
                        Console.WriteLine($"1から{EquipmentList.Length + 1}の間の番号を入力してください。");
                    }
                }
                else
                {
                    Console.WriteLine("有効な番号を入力してください。");
                }
            }
        }

        /// <summary>
        /// 点検結果（OK/NG）を入力する
        /// </summary>
        /// <returns>点検結果（OK または NG）</returns>
        private static string InputInspectionResult()
        {
            Console.WriteLine("--- 点検結果入力 ---");
            
            while (true)
            {
                Console.Write("点検結果を入力してください (OK/NG): ");
                string? input = Console.ReadLine()?.Trim().ToUpper();
                
                if (input == "OK" || input == "NG")
                {
                    Console.WriteLine();
                    return input;
                }
                
                Console.WriteLine("OKまたはNGを入力してください。");
            }
        }

        /// <summary>
        /// コメント（任意）を入力する
        /// </summary>
        /// <returns>入力されたコメント（空の場合は「なし」）</returns>
        private static string InputComment()
        {
            Console.WriteLine("--- コメント入力（任意） ---");
            Console.Write("コメントを入力してください（Enter キーでスキップ）: ");
            string? comment = Console.ReadLine()?.Trim();
            Console.WriteLine();
            
            // コメントが空の場合は「なし」を返す
            return string.IsNullOrEmpty(comment) ? "なし" : comment;
        }

        /// <summary>
        /// 点検記録をCSVファイルに保存する
        /// ファイル名は日付付き（例：inspection_results_20260115.csv）
        /// </summary>
        /// <param name="dateTime">記録日時</param>
        /// <param name="equipmentName">設備名</param>
        /// <param name="result">点検結果</param>
        /// <param name="comment">コメント</param>
        private static void SaveToCSV(string dateTime, string equipmentName, string result, string comment)
        {
            // ファイル名を日付付きで生成（例：inspection_results_20260115.csv）
            string date = DateTime.Now.ToString("yyyyMMdd");
            string fileName = $"inspection_results_{date}.csv";
            
            // CSVの1行データを作成
            // カンマが含まれる可能性があるフィールドはダブルクォートで囲む
            string csvLine = $"{dateTime},{EscapeCSV(equipmentName)},{result},{EscapeCSV(comment)}";
            
            // ファイルが存在しない場合はヘッダー行を追加
            if (!File.Exists(fileName))
            {
                string header = "日時,設備名,点検結果,コメント";
                File.WriteAllText(fileName, header + Environment.NewLine, Encoding.UTF8);
            }
            
            // データ行を追記
            File.AppendAllText(fileName, csvLine + Environment.NewLine, Encoding.UTF8);
        }

        /// <summary>
        /// CSV用に文字列をエスケープする
        /// カンマやダブルクォートが含まれる場合は適切に処理
        /// </summary>
        /// <param name="value">エスケープする文字列</param>
        /// <returns>エスケープされた文字列</returns>
        private static string EscapeCSV(string value)
        {
            // カンマ、ダブルクォート、改行が含まれる場合はダブルクォートで囲む
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n') || value.Contains('\r'))
            {
                // ダブルクォートは2つに置き換える
                value = value.Replace("\"", "\"\"");
                return $"\"{value}\"";
            }
            return value;
        }
    }
}
