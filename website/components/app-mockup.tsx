import Image from "next/image";
import { LayoutGrid, Settings, BarChart3, Minus, Square, X } from "lucide-react";

const rows = [
  { title: "Uncapped FPS", desc: "DFIntTaskSchedulerTargetFps preset", on: true },
  { title: "Auto-restart on crash", desc: "Relaunch Roblox automatically", on: true },
  { title: "Friend activity panel", desc: "Opt-in, off by default", on: false },
];

const bars = [35, 60, 42, 78, 50, 90, 64, 70, 38, 55, 48, 82];

export function AppMockup() {
  return (
    <div className="h-full w-full overflow-hidden rounded-2xl border border-white/10 bg-gradient-to-b from-[#141420] to-[#0d0d15]">
      <div className="flex items-center justify-between border-b border-white/10 bg-white/[0.02] px-4 py-2.5">
        <div className="flex items-center gap-2 text-xs font-medium text-zinc-400">
          <Image src="/logo.png" alt="" width={16} height={16} className="rounded-[4px]" />
          RailStrap — Settings
        </div>
        <div className="flex items-center gap-3 text-zinc-500">
          <Minus className="h-3 w-3" />
          <Square className="h-2.5 w-2.5" />
          <X className="h-3 w-3" />
        </div>
      </div>

      <div className="flex h-[calc(100%-41px)]">
        <div className="flex w-14 flex-col items-center gap-5 border-r border-white/10 bg-white/[0.012] py-4">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-[#7c5cff]/15 text-[#a998ff]">
            <LayoutGrid className="h-[18px] w-[18px]" />
          </div>
          <div className="flex h-9 w-9 items-center justify-center rounded-lg text-zinc-600">
            <Settings className="h-[18px] w-[18px]" />
          </div>
          <div className="flex h-9 w-9 items-center justify-center rounded-lg text-zinc-600">
            <BarChart3 className="h-[18px] w-[18px]" />
          </div>
        </div>

        <div className="flex flex-1 flex-col gap-3.5 p-5">
          {rows.map((row) => (
            <div
              key={row.title}
              className="flex items-center justify-between gap-4 rounded-xl border border-white/10 bg-white/[0.015] px-4 py-3"
            >
              <div className="flex flex-col gap-0.5">
                <span className="text-[13px] font-semibold text-zinc-100">{row.title}</span>
                <span className="text-[11px] text-zinc-500">{row.desc}</span>
              </div>
              <div
                className={`relative h-[19px] w-[34px] flex-none rounded-full transition-colors ${
                  row.on ? "bg-gradient-to-br from-[#8a5bff] to-[#4f8cff]" : "bg-zinc-700"
                }`}
              >
                <span
                  className={`absolute top-[2px] h-[15px] w-[15px] rounded-full bg-white transition-all ${
                    row.on ? "left-[17px]" : "left-[2px]"
                  }`}
                />
              </div>
            </div>
          ))}

          <div className="flex h-11 items-end gap-1 px-0.5">
            {bars.map((h, i) => (
              <span
                key={i}
                className="flex-1 rounded-t-sm bg-gradient-to-b from-[#8a5bff] to-[#4f8cff]/30"
                style={{ height: `${h}%` }}
              />
            ))}
          </div>

          <div className="flex flex-wrap gap-2">
            {["Ping 24ms", "Server hop", "Studio plugins"].map((p) => (
              <span
                key={p}
                className="rounded-md border border-[#7c5cff]/25 bg-[#7c5cff]/10 px-2.5 py-1 text-[10.5px] font-semibold text-[#b7a9ff]"
              >
                {p}
              </span>
            ))}
          </div>
        </div>
      </div>
    </div>
  );
}
