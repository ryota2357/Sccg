# メモ

## vim/neovim の help から Source の Group を作成する

`tmp.txt` に help の一部があるとする。

事前に`tmp.txt`から余分な行(HighlightGroupに関係ないところ)を削除しておく。

```shell
cat tmp.txt |\
  sed -e 's/ *$//' -e 's/^ /\t/' |\
  tr -s '\t' |\
  tr -s ' ' |\
  sed 's/^\t\*.*\*$//' |\
  tr -s '\n' |\
  gsed -z 's/\n\t/ /g' |\
  awk -F '\t' -v 'OFS=\n' '{
     sub("^","\n/// <summary>\n/// ",$2);
     sub("$","\n/// </summary>",$2);
     sub("$",",",$1)
     print $2,$1 }'
```

1. `sed -e 's/ *$//' -e 's/^ /\t/'` で行末の空白を削除 + 行頭の空白をタブに置換
2. `tr -s '\t'` でタブを 1 つに圧縮
3. `tr -s ' '` でスペースを 1 つに圧縮
4. `sed 's/^\t\*.*\*$//'` で `(タブ)*~~~*` を削除 (help のタグ行を削除)
5. `tr -s '\n'` で空行を削除
6. `gsed -z 's/\n\t/ /g'` でタブ始まりの行をjoin (複数行の説明を1行にする)
7. ここまでで、1列目にGroup名、2列目に説明がある tsv になっているので、`awk ...'` で C# の XML コメントに変換