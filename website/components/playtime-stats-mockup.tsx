import Image from "next/image";
import {
  Plug,
  Play,
  Wrench,
  Sparkles,
  Puzzle,
  BarChart3,
  Flag,
  Paintbrush,
  LayoutGrid,
  Settings,
  HelpCircle,
  Minus,
  Square,
  X,
} from "lucide-react";

const navItems = [
  { icon: Plug, label: "Integrations" },
  { icon: Play, label: "Bootstrapper" },
  { icon: Wrench, label: "Mods" },
  { icon: Sparkles, label: "Gallery" },
  { icon: Puzzle, label: "Studio Plugins" },
  { icon: BarChart3, label: "Playtime Stats", active: true },
  { icon: Flag, label: "Engine Settings" },
  { icon: Paintbrush, label: "Appearance" },
  { icon: LayoutGrid, label: "Shortcuts" },
];

const footerItems = [
  { icon: Settings, label: "RailStrap" },
  { icon: HelpCircle, label: "About" },
];

export function PlaytimeStatsMockup() {
  return (
    <div className="flex h-full w-full flex-col overflow-hidden rounded-md border border-white/10 bg-[#0c0c12]">
      <div className="flex items-center justify-between border-b border-white/10 bg-white/[0.02] px-3 py-2">
        <div className="flex items-center gap-2 text-[11px] font-medium text-zinc-400">
          <Image src="/logo.png" alt="" width={14} height={14} className="rounded-[3px]" />
          RailStrap Settings
        </div>
        <div className="flex items-center gap-3 text-zinc-500">
          <Minus className="h-3 w-3" />
          <Square className="h-2.5 w-2.5" />
          <X className="h-3 w-3" />
        </div>
      </div>

      <div className="flex min-h-0 flex-1">
        <div className="flex w-[132px] flex-none flex-col gap-0.5 border-r border-white/10 bg-white/[0.012] p-2.5">
          {navItems.map((item) => (
            <div
              key={item.label}
              className={`flex items-center gap-2 rounded-md px-2 py-1.5 text-[10.5px] font-medium ${
                item.active ? "bg-[#7c5cff]/15 text-[#c3b6ff]" : "text-zinc-500"
              }`}
            >
              <item.icon className="h-3 w-3 flex-none" strokeWidth={2} />
              <span className="truncate">{item.label}</span>
            </div>
          ))}
          <div className="mt-auto flex flex-col gap-0.5 border-t border-white/10 pt-2">
            {footerItems.map((item) => (
              <div key={item.label} className="flex items-center gap-2 px-2 py-1.5 text-[10.5px] font-medium text-zinc-500">
                <item.icon className="h-3 w-3 flex-none" strokeWidth={2} />
                <span className="truncate">{item.label}</span>
              </div>
            ))}
          </div>
        </div>

        <div className="flex-1 overflow-hidden p-4">
          <h4 className="text-[15px] font-bold text-zinc-100">Playtime Stats</h4>
          <p className="mt-1 text-[10.5px] leading-relaxed text-zinc-500">
            See how much time you&apos;ve spent in each game, tracked locally on this device.
          </p>

          <div className="mt-3 flex items-center justify-between gap-3 rounded-lg border border-white/10 bg-white/[0.02] px-3 py-2.5">
            <div>
              <div className="text-[11px] font-semibold text-zinc-100">Track playtime</div>
              <div className="text-[10px] text-zinc-500">
                Session lengths are recorded locally and never leave your device.
              </div>
            </div>
            <div className="relative h-[15px] w-[27px] flex-none rounded-full bg-gradient-to-br from-[#8a5bff] to-[#4f8cff]">
              <span className="absolute left-[13px] top-[2px] h-[11px] w-[11px] rounded-full bg-white" />
            </div>
          </div>

          <div className="mt-4 flex h-24 items-center justify-center rounded-lg border border-dashed border-white/10 text-center text-[10.5px] text-zinc-600">
            No sessions recorded yet. Play something!
          </div>
        </div>
      </div>

      <div className="flex items-center justify-between border-t border-white/10 px-3 py-2">
        <div className="flex items-center gap-1.5">
          <div className="h-[13px] w-[24px] rounded-full bg-zinc-700" />
          <span className="text-[10px] text-zinc-500">Test mode</span>
        </div>
        <div className="flex items-center gap-1.5">
          <div className="rounded-md bg-[#4f8cff] px-2.5 py-1 text-[10px] font-semibold text-white">Save</div>
          <div className="rounded-md border border-white/10 px-2.5 py-1 text-[10px] font-semibold text-zinc-400">
            Close
          </div>
        </div>
      </div>
    </div>
  );
}
