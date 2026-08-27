import { ArrowDownToLine } from "lucide-react";

export function DownloadCTA({ version }: { version: string }) {
  return (
    <section id="download" className="px-6 py-20 sm:py-24">
      <div className="mx-auto flex max-w-xl flex-col items-center text-center">
        <p className="font-mono text-[10px] font-bold uppercase tracking-[0.22em] text-[#8f7cff]">
          Ready to launch?
        </p>
        <h2 className="mt-3 text-3xl font-extrabold tracking-[-0.04em] sm:text-4xl">
          Get RailStrap.
        </h2>

        <a
          href="https://github.com/Yoursel71/RailStrap/releases/latest"
          target="_blank"
          rel="noopener noreferrer"
          className="group mt-8 inline-flex h-14 items-center gap-3 rounded-full bg-gradient-to-r from-[#8f72ff] to-[#558cff] px-7 text-sm font-bold text-white shadow-[0_16px_45px_-16px_rgba(107,91,255,.9)] transition-transform hover:-translate-y-0.5 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-[#9c87ff] focus-visible:ring-offset-4 focus-visible:ring-offset-background"
        >
          <ArrowDownToLine className="h-4 w-4 transition-transform group-hover:translate-y-0.5" />
          Download v{version}
        </a>

        <p className="mt-4 text-[11px] text-zinc-600">Windows 10/11 · .NET 6</p>
      </div>
    </section>
  );
}
