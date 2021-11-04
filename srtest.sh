path="./tests/sr"
in="$path/1.in"
out="$path/1.out"
expected="$path/1.expected"

sh program.sh -sr "$in" > "$out"
DIFF=$(diff "$out" "$expected")
if ["$DIFF" != ""]
then
	echo "out==expected"
else
	echo "out!=expected"
fi

