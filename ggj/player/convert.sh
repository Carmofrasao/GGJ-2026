for i in original/*.png; do
  output=$(basename $i)
  convert -resize x216 $i $output
done
